using UnityEngine;

namespace Game.Views
{
    public sealed class EndGameViewCollection : MonoBehaviour
    {
        public EndGameView[] EndGameViews;

        private void OnValidate()
        {
            EndGameViews = GetComponentsInChildren<EndGameView>();
        }
    }
}