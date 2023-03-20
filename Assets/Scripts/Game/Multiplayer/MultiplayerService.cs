using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Events;
using Game.Multiplayer;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using VContainer.Unity;

namespace Game.Core
{
    public sealed class MultiplayerService : IStartable, ITickable
    {
        private const int kCientsToPlay = 2;

        private const string kRelayCode = "relayCode"; 

        private ISignalBus _signalBus;

        private MultiplayerTaskMetadata _loginSequenceTask;

        private NetworkManager _networkManager;

        private Lobby _createdLobby;

        public void Start()
        {
            _signalBus.Subscribe<InitializeGameEvent>(this, OnInitializeGame);
            _signalBus.Subscribe<AllPlayersConnected>(this, OnConnectionEstablished);
        }

        private void OnConnectionEstablished(AllPlayersConnected obj)
        {
            if (_createdLobby != null)
            {
                try
                {
                    LobbyService.Instance.DeleteLobbyAsync(_createdLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private void OnInitializeGame(InitializeGameEvent obj)
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                Task = UnityServices.InitializeAsync(),
                Stage = MultiplayerJoinSequence.InitializeUnityServices
            };
        }

        public MultiplayerService(ISignalBus signalBus, NetworkManager networkManager)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
        }

        public void Tick()
        {
            if (_loginSequenceTask == null || !_loginSequenceTask.Task.IsCompleted)
            {
                return;
            }

            OnTaskCompleted(_loginSequenceTask);
        }

        private void OnTaskCompleted(MultiplayerTaskMetadata completedMultiplayerTask)
        {
            if (completedMultiplayerTask.Task.IsFaulted)
            {
                Debug.LogError(
                    $"Multiplayer sequence failed with error on stage {completedMultiplayerTask.Stage}." +
                    $" Error {completedMultiplayerTask.Task.Exception}");
                return;
            }

            switch (completedMultiplayerTask.Stage)
            {
                case MultiplayerJoinSequence.InitializeUnityServices:
                    _loginSequenceTask = new MultiplayerTaskMetadata
                    {
                        Stage = MultiplayerJoinSequence.SignIn,
                        Task = AuthenticationService.Instance.SignInAnonymouslyAsync()
                    };
                    break;

                case MultiplayerJoinSequence.SignIn:

                    var options = new QueryLobbiesOptions
                    {
                        Filters = new List<QueryFilter>
                        {
                            new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                        }
                    };

                    _loginSequenceTask = new MultiplayerTaskMetadata
                    {
                        Stage = MultiplayerJoinSequence.ListLobbies,
                        Task = LobbyService.Instance.QueryLobbiesAsync(options)
                    };

                    break;

                case MultiplayerJoinSequence.ListLobbies:
                    var lobbyQuery = _loginSequenceTask.Task as Task<QueryResponse>;
                    if (lobbyQuery == null)
                    {
                        ConnectionError($"Invalid cast: {_loginSequenceTask.Task.GetType()} to Task<QueryResponse>");
                        return;
                    }

                    var lobbyList = lobbyQuery.Result.Results;
                    if (lobbyList.Count > 0)
                    {
                        JoinLobby(lobbyList[0]);
                    }
                    else
                    {
                        CreateRelayAllocation();
                    }
                    break;

                case MultiplayerJoinSequence.CreateRelayAllocation:
                    var relayAllocation = _loginSequenceTask.Task as Task<Allocation>;
                    if (relayAllocation == null)
                    {
                        ConnectionError($"Invalid cast: {_loginSequenceTask.Task.GetType()} to Task<Allocation>");
                        return;
                    }

                    ConnectToRelayHost(relayAllocation.Result);
                    GetAllocationJoinCode(relayAllocation.Result);
                    break;

                case MultiplayerJoinSequence.GetAllocationJoinCode:
                    var relayJoinCode = _loginSequenceTask.Task as Task<string>;
                    if (relayJoinCode == null)
                    {
                        ConnectionError($"Invalid cast: {_loginSequenceTask.Task.GetType()} to Task<string>");
                        return;
                    }
                    CreateLobby(relayJoinCode.Result);
                    break;

                case MultiplayerJoinSequence.CreateLobby:
                    _loginSequenceTask = null;
                    break;

                case MultiplayerJoinSequence.JoinLobby:
                    var joinedLobby = _loginSequenceTask.Task as Task<Lobby>;
                    if (joinedLobby == null)
                    {
                        ConnectionError($"Invalid cast: {_loginSequenceTask.Task.GetType()} to Task<Lobby>");
                        return;
                    }

                    var relayCode = joinedLobby.Result.Data[kRelayCode];
                    JoinAllocation(relayCode);
                    break;

                case MultiplayerJoinSequence.JoinAllocation:
                    var joinedAllocation = _loginSequenceTask.Task as Task<JoinAllocation>;
                    if (joinedAllocation == null)
                    {
                        ConnectionError($"Invalid cast: {_loginSequenceTask.Task.GetType()} to Task<JoinAllocation>");
                        return;
                    }
                    _loginSequenceTask = null;
                    ConnectToRelayClient(joinedAllocation.Result);
                    break;

                default:
                    ConnectionError("Unknown stage error");
                    break;
            }
        }

        private void ConnectToRelayHost(Allocation allocation)
        {
            _networkManager.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            _networkManager.StartHost();
        }

        private void ConnectToRelayClient(JoinAllocation allocation)
        {
            _networkManager.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            _networkManager.StartClient();
        }

        private void JoinAllocation(DataObject data)
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                Task = RelayService.Instance.JoinAllocationAsync(data.Value),
                Stage = MultiplayerJoinSequence.JoinAllocation
            };
        }

        private void CreateRelayAllocation()
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                // Host does not count as a player here, so -1
                Task = RelayService.Instance.CreateAllocationAsync(kCientsToPlay),
                Stage = MultiplayerJoinSequence.CreateRelayAllocation
            };
        }

        private void GetAllocationJoinCode(Allocation allocation)
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                Task = RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId),
                Stage = MultiplayerJoinSequence.GetAllocationJoinCode
            };
        }

        private void CreateLobby(string relayJoinCode)
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                Stage = MultiplayerJoinSequence.CreateLobby,
                Task = LobbyService.Instance.CreateLobbyAsync("0", kCientsToPlay, new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {kRelayCode, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                    }
                })
            };
        }

        private void JoinLobby(Lobby lobby)
        {
            _loginSequenceTask = new MultiplayerTaskMetadata
            {
                Task = LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id),
                Stage = MultiplayerJoinSequence.JoinLobby
            };
        }

        private void ConnectionError(string error)
        {
            _loginSequenceTask = null;
            _signalBus.Publish(new MultiplayerConnectionError(error));
        }
    }
}