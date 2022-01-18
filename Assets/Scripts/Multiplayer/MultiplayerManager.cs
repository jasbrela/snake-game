using System.Collections.Generic;
using Enums;
using Snake;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace Multiplayer
{
    public class MultiplayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject snakePrefab;
        [SerializeField] private Transform cardsParent;
        [SerializeField] private Transform newCard;
        [SerializeField] private InputAction excludeBindings;
        
        private readonly List<SnakeInformation> _snakeInfos = new List<SnakeInformation>();
        private SnakeInformation _currentInfo;
        
        private void OnEnable()
        {
            InputManager.Instance.RebindCanceled += RemovePlayerFromCancelledRebind;
        }

        private void OnDisable()
        {
            InputManager.Instance.RebindCanceled -= RemovePlayerFromCancelledRebind;
        }

        /// <summary>
        /// Add new player.
        /// </summary>
        public void AddPlayer()
        {
            InputManager.Instance.RebindComplete += StartNextRebind;
            InputManager.Instance.RebindComplete -= OnSuccessfullyAddedNewPlayer;

            _currentInfo = new SnakeInformation(_snakeInfos.Count + 1);
            CreateCard();
        }

        /// <summary>
        /// Create the Player's card
        /// </summary>
        private void CreateCard()
        {
            GameObject card = Instantiate(snakePrefab, transform.position, Quaternion.identity);
            card.transform.SetParent(cardsParent);
        
            _currentInfo.Card = card;
            _currentInfo.Color = GetRandomColor();
        
            _currentInfo.SetManager(card.GetComponent<SnakeManager>());
            _currentInfo.Manager.OnClickDelete += RemovePlayerFromCard;

            _currentInfo.Input = _currentInfo.Manager.GetHead().GetComponent<PlayerInput>();
            InputUser.PerformPairingWithDevice(Keyboard.current, _currentInfo.Input.user);

            _snakeInfos.Add(_currentInfo);
        
            InputManager.Instance.StartRebind(_currentInfo.Input, InputActions.TurnLeft.ToString(),
                0, excludeBindings);
        }
        
        /// <summary>Get a random vivid color</summary>
        /// <returns>A random color</returns>
        private Color GetRandomColor()
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        }
    
        /// <summary>
        /// Remove the Player's card when the rebind process is cancelled.
        /// </summary>
        private void RemovePlayerFromCancelledRebind()
        {
            _snakeInfos[_currentInfo.ID - 1].Input.user.UnpairDevicesAndRemoveUser();
            Destroy(_snakeInfos[_currentInfo.ID - 1].Card);
            _snakeInfos.RemoveAt(_currentInfo.ID - 1);
        }
    
        /// <summary>
        /// Remove the Player's card when the Player press the Close Button and updates the other cards.
        /// </summary>
        /// <param name="index">Player's index</param>
        private void RemovePlayerFromCard(int index)
        {
            foreach (InputAction action in _snakeInfos[index].Input.actions)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    InputManager.Instance.MakeBindingAvailable(binding);
                }
            }

            for (int i = index; i < _snakeInfos.Count; i++)
            {
                _snakeInfos[i].Manager.UpdateID(i);
            }
            
            _snakeInfos[index].Input.user.UnpairDevicesAndRemoveUser();
            Destroy(_snakeInfos[index].Card);
            _snakeInfos.Remove(_snakeInfos[index]);
            
        }

        /// <summary>
        /// Start the rebinding process for the TurnRight key.
        /// </summary>
        private void StartNextRebind()
        {
            InputManager.Instance.RebindComplete -= StartNextRebind;
            InputManager.Instance.RebindComplete += OnSuccessfullyAddedNewPlayer;
            InputManager.Instance.StartRebind(_currentInfo.Input, InputActions.TurnRight.ToString(),
                0, excludeBindings);
        }

        /// <summary>
        /// Called when a player is successfully added, after passing on both rebinds.
        /// </summary>
        private void OnSuccessfullyAddedNewPlayer()
        {
            _currentInfo.Manager.ShowCard();
            newCard.SetAsLastSibling();
        }
    
    }
}
