using System.Collections.Generic;
using Enums;
using Snake;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Multiplayer
{
    public class MultiplayerManager : MonoBehaviour
    {
        [Header("Game Information")]
        [Range(1, 20)][SerializeField]  private int minimumPlayers;
        
        [Header("Cards Information")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform cardsParent;
        [SerializeField] private Transform newCard;
        [SerializeField] private Button newCardButton;
        
        [Header("UI")]
        [SerializeField] private Button startGameButton;
        
        [Header("Rebinding")]
        [SerializeField] private Popup popup;
        [SerializeField] private IgnoredBindings ignored;
        
        private readonly List<SnakeInformation> _snakeInfos = new List<SnakeInformation>();
        private SnakeInformation _currentInfo;

        private void OnEnable() => ListenToEvents();
        private void OnDisable() => StopListeningToEvents();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            }

        /// <summary>
        /// Listen to events
        /// </summary>
        private void ListenToEvents()
        {
            InputManager.Instance.RebindCanceled += OnRebindCanceled;
            InputManager.Instance.OnDuplicateFound += popup.ShowDuplicateMessage;
        }
        
        /// <summary>
        /// Stop listening to events.
        /// </summary>
        private void StopListeningToEvents()
        {
            InputManager.Instance.RebindCanceled -= RemovePlayerFromCancelledRebind;
            InputManager.Instance.OnDuplicateFound -= popup.ShowDuplicateMessage;
        }

        /// <summary>
        /// Add new player.
        /// </summary>
        public void AddPlayer()
        {
            newCardButton.interactable = false;
            startGameButton.interactable = false;
            popup.OnAddNewPlayerShowMessage();
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
            GameObject card = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            card.transform.SetParent(cardsParent);
            card.name = $"Adding new player...";

            _currentInfo.Card = card;
            _currentInfo.Color = GetRandomColor();

            _currentInfo.SetManager(card.GetComponent<SnakeManager>());
            _currentInfo.Manager.OnClickDelete += RemovePlayerFromCard;

            _currentInfo.Input = _currentInfo.Manager.GetHead().GetComponent<PlayerInput>();
            InputUser.PerformPairingWithDevice(Keyboard.current, _currentInfo.Input.user);

            _snakeInfos.Add(_currentInfo);

            InputManager.Instance.StartRebind(_currentInfo.Input, InputActions.TurnLeft.ToString(),
                0, ignored.bindings);
        }

        /// <summary>Get a random vivid color</summary>
        /// <returns>A random color</returns>
        private Color GetRandomColor()
        {
            return Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        }

        /// <summary>
        /// Calls the methods when the rebind operation is canceled.
        /// </summary>
        private void OnRebindCanceled()
        {
            RemovePlayerFromCancelledRebind();
            newCardButton.interactable = true;
            startGameButton.interactable = true;
            popup.HidePopup();
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
                var info = _snakeInfos[i];

                info.Manager.UpdateID(i);
                info.Card.name = $"P{info.ID}'s Card";
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
                0, ignored.bindings);
        }

        /// <summary>
        /// Called when a player is successfully added, after passing on both rebinds.
        /// </summary>
        private void OnSuccessfullyAddedNewPlayer()
        {
            popup.HidePopup();
            newCardButton.interactable = true;
            startGameButton.interactable = true;

            var info = _snakeInfos[_snakeInfos.Count - 1];
            info.Card.name = $"P{info.ID}'s Card";
            
            _currentInfo.Manager.ShowCard();
            newCard.SetAsLastSibling();
        }

        /// <summary>
        /// If there's enough player, start the game. Otherwise, show a warning.
        /// </summary>
        public void OnClickStart()
        {
            if (_snakeInfos.Count < minimumPlayers)
            {
                popup.ShowNotEnoughPlayersMessage(minimumPlayers);
                return;
            }
            
            // TODO: Prepare the snakes to start the game.
            
            foreach (SnakeInformation snake in _snakeInfos)
            {
                snake.Card.transform.SetParent(transform);
                snake.Manager.ShowHead();
            }
            
            //SceneManager.LoadScene(Scenes.MultiplayerGame.ToString());
        }
    }
}
