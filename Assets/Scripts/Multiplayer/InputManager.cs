using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Multiplayer
{
    public class InputManager : MonoBehaviour
    {
        #region Singleton
        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Input Manager");
                    go.AddComponent<InputManager>();
                }
                return _instance;
            }
        }
    
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        public event Action RebindComplete;
        public event Action RebindCanceled;
        public event Action<InputAction, int> RebindStarted;

        private void OnDestroy()
        {
            RebindComplete = null;
            RebindCanceled = null;
            RebindStarted = null;
        }

        /// <summary>
        /// Start the rebind process.
        /// </summary>
        /// <param name="input">The desired PlayerInput's to rebind.</param>
        /// <param name="actionName">The desired action's name to rebind.</param>
        /// <param name="index">The desired binding's index to rebind.</param>
        public void StartRebind(PlayerInput input, string actionName, int index)
        {
            // TODO: Check for duplicates
            
            InputAction action = input.actions.FindAction(actionName);
            if (action == null || action.bindings.Count <= index)
            {
                Debug.LogWarning("Couldn't find action or binding.");
                return;
            }

            if (action.bindings[index].isComposite)
            {
                var firstPartIndex = index + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
                {
                    Rebind(action, index, true);
                }
            }
            else
            {
                Rebind(action, index, false);
            }
        }

        /// <summary>
        /// Rebind
        /// </summary>
        /// <param name="action">The desired InputAction to rebind.</param>
        /// <param name="index">The desired binding's index to rebind.</param>
        /// <param name="allCompositeParts">Should rebind all composite parts?</param>
        private void Rebind(InputAction action, int index, bool allCompositeParts)
        {
            if (action == null || index < 0) return;
            
            // TODO: Show POPUP
            Debug.Log($"Press a {action.expectedControlType} key");
        
            action.Disable();
            var rebind = action.PerformInteractiveRebinding(index);

            rebind.OnComplete(operation =>
            {
                Debug.Log("Finished");

                action.Enable();
                operation.Dispose();

                if (allCompositeParts)
                {
                    var nextIndex = index + 1;
                    if (nextIndex < action.bindings.Count && action.bindings[nextIndex].isComposite)
                    {
                        Rebind(action, nextIndex, true);
                    }
                }

                RebindComplete?.Invoke();
            });

            rebind.OnCancel(operation =>
            {
                Debug.Log("Canceled");

                action.Enable();
                operation.Dispose();
            
                RebindCanceled?.Invoke();
            });

            rebind.WithControlsExcluding("Mouse");
            rebind.WithControlsExcluding("Keyboard/Alt");
        
            RebindStarted?.Invoke(action, index);
            rebind.Start();
        }

        /// <summary>
        /// Get the binding name.
        /// </summary>
        /// <param name="input">The binding's PlayerInput</param>
        /// <param name="actionName">The binding's action</param>
        /// <param name="index">The binding's index</param>
        /// <param name="options">Desired DisplayStringOptions</param>
        /// <returns></returns>
        public string GetBindingName(PlayerInput input, string actionName, int index, InputBinding.DisplayStringOptions options)
        {
            InputAction action = input.actions.FindAction(actionName);
            return action.GetBindingDisplayString(index, options);
        }
    }
}