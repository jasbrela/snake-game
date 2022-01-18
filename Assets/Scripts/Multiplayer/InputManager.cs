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
        public event Action OnDuplicateFound;
        public event Action<InputAction, int> RebindStarted;
        
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
            
            // TODO: Show POPUP
            Debug.Log($"Press a {action.expectedControlType} key");
        
            action.Disable();
            var rebind = action.PerformInteractiveRebinding(index);

            rebind.OnComplete(operation =>
            {
                Debug.Log("Finished");

                action.Enable();
                
                if (CheckDuplicates(action, index, allCompositeParts))
                {
                    action.RemoveBindingOverride(index);
                    operation.Dispose();
                    Rebind(action, index, allCompositeParts, excludeBindings);
                    return;
                }

                unavailableKeys.Add(action.bindings[index]);
                
                operation.Dispose();

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

            rebind.OnCancel(operation =>
            {
                Debug.Log("Canceled");

                action.Enable();
                operation.Dispose();
            
                RebindCanceled?.Invoke();
            });

            ExcludeControls(ref rebind, excludeBindings);

            RebindStarted?.Invoke(action, index);
            rebind.Start();
        }

        /// <summary>
        /// Excludes the keys from a InputAction.
        /// </summary>
        /// <param name="op">The operation that should ignore the bindings.</param>
        /// <param name="action">The Input Action that contains the excluding bindings.</param>
        private void ExcludeControls(ref InputActionRebindingExtensions.RebindingOperation op, InputAction action)
        {
            foreach (InputBinding binding in action.bindings)
            {
                if (binding.effectivePath != null) continue;
                op.WithControlsExcluding(binding.effectivePath);
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
                OnDuplicateFound?.Invoke();
                return true;
            }

            if (allCompositeParts)
            {
                for (int i = 1; i < index; ++i)
                {
                    if (action.bindings[i].effectivePath != newBinding.effectivePath) continue;
                
                    Debug.Log("Duplicate found: " + newBinding.effectivePath);
                    OnDuplicateFound?.Invoke();
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