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
        [SerializeField] private GameObject AIPrefab;
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
        private void Awake() => DontDestroyOnLoad(this);
        

        private void Start()
        {
            GameManager.SetMultiplayerGame();
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
            if (newCardButton.interactable == false && _snakeInfos.Count < 12) newCardButton.interactable = true;
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
            if (newCardButton.interactable == false && _snakeInfos.Count < 12) newCardButton.interactable = true;
        }
        /// <summary>
        /// Removes the player after his snake dies.
        /// </summary>
        /// <param name="manager">The player's snake's Snake Manager.</param>
        private void RemovePlayerFromDeath(SnakeManager manager)
        {
            // TODO: Add game over
            
            _snakeInfos[manager.Info.ID - 1].Input.user.UnpairDevicesAndRemoveUser();
            Destroy(_snakeInfos[manager.Info.ID - 1].Card);
        }
        
        /// <summary>
        /// Destroy the AI snake when it dies.
        /// </summary>
        /// <param name="parent">The AI snake's parent</param>
        private void RemoveAIFromDeath(GameObject parent)
        {
            Destroy(parent);
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

            if (_snakeInfos.Count == 12) newCardButton.interactable = false;
        }

        /// <summary>
        /// If there's enough player, start the game. Otherwise, show a warning.
        /// </summary>
        public void OnClickStart()
        {
            StopListeningToEvents();
            SceneManager.sceneLoaded += SetUpSnakes;

            if (_snakeInfos.Count < minimumPlayers)
            {
                popup.ShowNotEnoughPlayersMessage(minimumPlayers);
                return;
            }
            
            foreach (SnakeInformation snake in _snakeInfos)
            {
                snake.Card.transform.SetParent(transform);
                snake.Card.transform.position = Vector3.zero;
                snake.Card.name = $"P{snake.ID}";
                snake.Manager.ShowHead();
            }
            
            LoadMap();
        }

        /// <summary>
        /// Loads the correct map, based on the player's quantity.
        /// </summary>
        private void LoadMap()
        {
            if (_snakeInfos.Count <= 3)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameSmall.ToString());
            }
            else if (_snakeInfos.Count <= 6)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameMedium.ToString());
            }
            else if (_snakeInfos.Count <= 9)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameLarge.ToString());
            }
            else if (_snakeInfos.Count <= 12)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameExtraLarge.ToString());
            }
        }

        /// <summary>
        /// Set up the snakes on load map.
        /// </summary>
        private void SetUpSnakes(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= SetUpSnakes;
            
            if (!scene.name.Contains("Multiplayer")) return;
            
            foreach (SnakeInformation snake in _snakeInfos)
            {
                GameObject ai = Instantiate(AIPrefab, transform);
                SnakeController aiController = ai.GetComponentInChildren<SnakeController>();
                aiController.SetColor(GetAIColor(snake.Color));
                aiController.SendOnAISnakeDieCallback(RemoveAIFromDeath);
                
                snake.Manager.EnableSnakeController();
                snake.Manager.GetSnakeController().SendOnPlayerSnakeDieCallback(RemovePlayerFromDeath);
                snake.Manager.GetSnakeController().SetColor(snake.Color);
                snake.Manager.GetSnakeController().ResetSnake();
            }

            GameManager.Instance.Retry();
        }

        /// <summary>
        /// Get a color for the AI snake. It's a lighter version of the player's snake.
        /// </summary>
        /// <param name="color">Player snake's color</param>
        /// <returns>A lighter color.</returns>
        private Color GetAIColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            s = .25f;
            return Color.HSVToRGB(h, s, v);
        }
    }
}
