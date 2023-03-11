using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
   public sealed class NetworkManagerUI : MonoBehaviour
   {
      [SerializeField] private Button _hostButton;
      [SerializeField] private Button _clientButton;

      private void Start()
      {
         _hostButton.onClick.AddListener(() =>
         {
            NetworkManager.Singleton.StartHost();
         });

         _clientButton.onClick.AddListener(() =>
         {
            NetworkManager.Singleton.StartClient();
         });
      }
   }
}