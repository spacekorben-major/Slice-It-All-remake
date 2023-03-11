using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _startSolo;

        public Button StartSolo => _startSolo;
    }
}