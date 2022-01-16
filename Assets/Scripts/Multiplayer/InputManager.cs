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

        public string GetBindingName(PlayerInput input, string actionName, int index, InputBinding.DisplayStringOptions options)
        {
            InputAction action = input.actions.FindAction(actionName);
            return action.GetBindingDisplayString(index, options);
        }
    }
}