using Unity.Netcode.Components;

namespace Game.Utils
{
    public class ClientNetworkTransform : NetworkTransform
    {
        /// <summary>
        /// Since it's a casual game we will trust client to send correct knife position
        /// </summary>
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}