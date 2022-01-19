using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
                if (_instance != null) return _instance;
                
                GameObject go = new GameObject("Input Manager");
                go.AddComponent<InputManager>();
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
        public event Action<string> OnDuplicateFound;
        public event Action<InputAction, int> RebindStarted;

        private InputActionRebindingExtensions.RebindingOperation operation;
        private readonly List<InputBinding> unavailableKeys = new List<InputBinding>();

        private void OnDestroy()
        {
            RebindComplete = null;
            RebindCanceled = null;
            RebindStarted = null;
            OnDuplicateFound = null;
        }

        /// <summary>
        /// Start the rebind process.
        /// </summary>
        /// <param name="input">The desired PlayerInput's to rebind.</param>
        /// <param name="actionName">The desired action's name to rebind.</param>
        /// <param name="index">The desired binding's index to rebind.</param>
        /// <param name="excludeBindings">A InputAction containing all the bindings to exclude.</param>
        public void StartRebind(PlayerInput input, string actionName, int index, [CanBeNull] InputAction excludeBindings)
        {
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
                    Rebind(action, index, true, excludeBindings);
                }
            }
            else
            {
                Rebind(action, index, false, excludeBindings);
            }
        }

        /// <summary>
        /// Rebind
        /// </summary>
        /// <param name="action">The desired InputAction to rebind.</param>
        /// <param name="index">The desired binding's index to rebind.</param>
        /// <param name="allCompositeParts">Should rebind all composite parts?</param>
        /// <param name="excludeBindings">A InputAction containing all the bindings to exclude.</param>
        private void Rebind(InputAction action, int index, bool allCompositeParts, [CanBeNull] InputAction excludeBindings)
        {
            if (action == null || index < 0) return;
            
            action.Disable();
            InputSystem.EnableDevice(Mouse.current);
            
            operation = action.PerformInteractiveRebinding(index);

            operation.OnComplete(op =>
            {
                Debug.Log("Finished");

                action.Enable();
                
                if (CheckDuplicates(action, index, allCompositeParts))
                {
                    action.RemoveBindingOverride(index);
                    op.Dispose();
                    Rebind(action, index, allCompositeParts, excludeBindings);
                    return;
                }

                unavailableKeys.Add(action.bindings[index]);
                
                op.Dispose();

                if (allCompositeParts)
                {
                    var nextIndex = index + 1;
                    if (nextIndex < action.bindings.Count && action.bindings[nextIndex].isComposite)
                    {
                        Rebind(action, nextIndex, true, excludeBindings);
                    }
                }

                RebindComplete?.Invoke();
            });

            operation.OnCancel(op =>
            {
                Debug.Log("Canceled");
                // BUG: After canceling, a non-existent duplicate is being detected
                //  1. Add a new player. There can be or not be more players, but I do recommend to have none.
                //  2. Press backspace to cancel.
                //  3. Add new player, again. Press one key, then another one. A duplicate will be detected
                // for the second one, but no popup will be shown.
                //  4. Press another key. The second key will be replaced, the UI updated and the game will
                // go back to normal.
                
                action.Enable();
                op.Dispose();
            
                RebindCanceled?.Invoke();
            });

            operation.WithCancelingThrough("<Keyboard>/backspace");
            ExcludeControls(excludeBindings);

            RebindStarted?.Invoke(action, index);
            operation.Start();
        }
        
        /// <summary>
        /// Excludes the keys from a InputAction.
        /// </summary>
        /// <param name="action">The Input Action that contains the excluding bindings.</param>
        private void ExcludeControls(InputAction action)
        {
            foreach (InputBinding binding in action.bindings)
            {
                // Will give you an error if there's a <No Binding> binding in excludeBindings.
                operation.WithControlsExcluding(binding.effectivePath);
            }
        }

        /// <summary>
        /// Check for duplicates.
        /// </summary>
        /// <param name="action">The desired InputAction to rebind.</param>
        /// <param name="index">The desired binding's index to rebind.</param>
        /// <param name="allCompositeParts">Should rebind all composite parts?</param>
        /// <returns>Returns 'true' if the chosen key is a duplicate. Otherwise returns 'false'.</returns>
        private bool CheckDuplicates(InputAction action, int index, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[index];
            foreach (InputBinding binding in unavailableKeys)
            {
                if (binding.effectivePath != newBinding.effectivePath) continue;
            
                Debug.Log("Duplicate found: " + newBinding.effectivePath);
                OnDuplicateFound?.Invoke(newBinding.effectivePath);
                return true;
            }

            if (allCompositeParts)
            {
                for (int i = 1; i < index; ++i)
                {
                    if (action.bindings[i].effectivePath != newBinding.effectivePath) continue;
                
                    Debug.Log("Duplicate found: " + newBinding.effectivePath);
                    OnDuplicateFound?.Invoke(newBinding.effectivePath);
                    return true;
                }
            }

            return false;
        }

        public void MakeBindingAvailable(InputBinding binding)
        {
            if (unavailableKeys.Contains(binding))
            {
                unavailableKeys.Remove(binding);
            }
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