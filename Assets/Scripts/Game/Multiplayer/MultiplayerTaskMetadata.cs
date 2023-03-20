using System.Threading.Tasks;
using Game.Multiplayer;

namespace Game.Core
{
    public class MultiplayerTaskMetadata
    {
        public Task Task;
        public MultiplayerJoinSequence Stage;
    }
}