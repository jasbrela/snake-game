using Snake;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Multiplayer
{
    public class InputUI : MonoBehaviour
    {
        [Header("Essentials")]
        [SerializeField] private SnakeManager snake;
        [SerializeField] private InputActionReference inputActionReference;
        [Range(0, 1)] [SerializeField] private int selectedBinding;
        [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI rebindText;

        private string _actionName;
        private int _bindingIndex;
        
        private void OnEnable()
        {
            InputManager.Instance.RebindComplete += UpdateUI;
            UpdateUI();
        }

        private void OnDisable()
        {
            InputManager.Instance.RebindComplete -= UpdateUI;
        }

        /// <summary>
        /// Get the binding's information based on selectedBinding.
        /// </summary>
        private void GetBindingInfo()
        {
            _actionName = inputActionReference.action.name;
        
            if (selectedBinding >= inputActionReference.action.bindings.Count) return;
            _bindingIndex = selectedBinding;
        }


        /// <summary>
        /// Updates the UI with the binding's information.
        /// </summary>
        private void UpdateUI()
        {
            if (inputActionReference == null) return;
        
            GetBindingInfo();
        
            if (rebindText == null) return;
        
            if (Application.isPlaying && snake != null)
            {
                rebindText.text =
                    InputManager.Instance.GetBindingName(snake.Info.Input, _actionName, _bindingIndex,
                        displayStringOptions);
            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(_bindingIndex, displayStringOptions);
            }
        }
    }
}