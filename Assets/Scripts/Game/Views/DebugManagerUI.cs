using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
   public sealed class DebugManagerUI : MonoBehaviour
   {
      [SerializeField] private Button _consoleButton;

      [SerializeField] private GameObject _console;

      private bool _active;

      private void Start()
      {
         _consoleButton.onClick.AddListener(() =>
         {
            _active = !_active;
            _console.SetActive(_active);
         });
      }
   }
}
