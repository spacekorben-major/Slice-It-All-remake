using UnityEngine;

namespace Game.Events
{
    public sealed class SlicedEvent : IGameEvent
    {
        public bool IsLocalPlayer; 

        public ContactPoint Contact;
    }
}