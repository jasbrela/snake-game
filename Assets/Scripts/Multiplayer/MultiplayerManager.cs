using System.Collections.Generic;
using Enums;
using JetBrains.Annotations;
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

        private readonly List<SnakeManager> _players = new List<SnakeManager>();
        private SnakeManager _currentPlayer;
        
        private List<SnakeManager> _alivePlayers = new List<SnakeManager>();
        private int snakesQuantity;

        private void OnEnable() => ListenToEvents();
        private void OnDisable() => StopListeningToEvents();
        private void Awake() => DontDestroyOnLoad(this);
        
        private void Start()
        {
            GameManager.SetMultiplayerGame(true);
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
        /// Clean the multiplayer settings.
        /// </summary>
        public void OnGoToMainMenu()
        {
            foreach (SnakeManager player in _players)
            {
                player.input.user.UnpairDevicesAndRemoveUser();
            }
            
            GameManager.SetMultiplayerGame(false);

            Destroy(gameObject);
            
            SceneManager.LoadScene(Scenes.MainMenu.ToString());
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

            _currentPlayer = card.GetComponent<SnakeManager>();
            _currentPlayer.ID = _players.Count + 1;
            _currentPlayer.Color = GetRandomColor();
            _currentPlayer.OnClickDelete += RemovePlayerFromCard;
            InputUser.PerformPairingWithDevice(Keyboard.current, _currentPlayer.input.user);

            _players.Add(_currentPlayer);

            InputManager.Instance.StartRebind(_currentPlayer.input, InputActions.TurnLeft.ToString(),
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
            InputManager.Instance.RebindComplete -= StartNextRebind;
            InputManager.Instance.RebindComplete -= OnSuccessfullyAddedNewPlayer;
            
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
            _players[_currentPlayer.ID - 1].input.user.UnpairDevicesAndRemoveUser();
            Destroy(_players[_currentPlayer.ID - 1].gameObject);
            _players.RemoveAt(_currentPlayer.ID - 1);
            if (newCardButton.interactable == false && _players.Count < 12) newCardButton.interactable = true;
        }
    
        /// <summary>
        /// Remove the Player's card when the Player press the Close Button and updates the other cards.
        /// </summary>
        /// <param name="index">Player's index</param>
        private void RemovePlayerFromCard(int index)
        {
            foreach (InputAction action in _players[index].input.actions)
            {
                foreach (InputBinding binding in action.bindings)
                {
                    InputManager.Instance.MakeBindingAvailable(binding);
                }
            }

            for (int i = index; i < _players.Count; i++)
            {
                var player = _players[i];

                player.UpdateID(i);
                player.name = $"P{player.ID}'s Card";
            }
            
            _players[index].input.user.UnpairDevicesAndRemoveUser();
            Destroy(_players[index].gameObject);
            _players.Remove(_players[index]);
            if (newCardButton.interactable == false && _players.Count < 12) newCardButton.interactable = true;
        }
        /// <summary>
        /// Subtract the player from alive list.
        /// </summary>
        /// <param name="manager">The player's snake's Snake Manager. Null if it's AI.</param>
        private void RemovePlayerFromDeath([CanBeNull] SnakeManager manager)
        {
            if (manager != null) _alivePlayers.Remove(manager);
            OnDeath();
        }

        /// <summary>
        /// Check if there's only one snake alive. If so, ends the game.
        /// </summary>
        private void OnDeath()
        {
            snakesQuantity--;
            
            if (_alivePlayers.Count == 0)
            {
                GameManager.Instance.EndGame();
            }
            
            if (_alivePlayers.Count == 1 && snakesQuantity == 1)
            {
                GameManager.Instance.OnHumanPlayerWins($"P{_alivePlayers[0].ID}", _alivePlayers[0].Color);
                GameManager.Instance.EndGame();
            }
        }

        /// <summary>
        /// Start the rebinding process for the TurnRight key.
        /// </summary>
        private void StartNextRebind()
        {
            InputManager.Instance.RebindComplete -= StartNextRebind;
            InputManager.Instance.RebindComplete += OnSuccessfullyAddedNewPlayer;
            InputManager.Instance.StartRebind(_currentPlayer.input, InputActions.TurnRight.ToString(),
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

            var player = _players[_players.Count - 1];
            player.name = $"P{player.ID}'s Card";
            
            _currentPlayer.ShowCard();
            newCard.SetAsLastSibling();

            if (_players.Count == 12) newCardButton.interactable = false;
        }

        /// <summary>
        /// If there's enough player, start the game. Otherwise, show a warning.
        /// </summary>
        public void OnClickStart()
        {
            if (_players.Count < minimumPlayers)
            {
                popup.ShowNotEnoughPlayersMessage(minimumPlayers);
                return;
            }
            
            StopListeningToEvents();
            SceneManager.sceneLoaded += OnLoadMap;
            
            foreach (SnakeManager player in _players)
            {
                player.transform.SetParent(transform);
                player.name = $"P{player.ID}";
                player.ShowHead();
            }
            
            LoadMap();
        }

        /// <summary>
        /// Loads the correct map, based on the player's quantity.
        /// </summary>
        private void LoadMap()
        {
            if (_players.Count <= 3)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameSmall.ToString());
            }
            else if (_players.Count <= 6)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameMedium.ToString());
            }
            else if (_players.Count <= 9)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameLarge.ToString());
            }
            else if (_players.Count <= 12)
            {
                SceneManager.LoadScene(Scenes.MultiplayerGameExtraLarge.ToString());
            }
        }

        /// <summary>
        /// Set up the snakes on load map.
        /// </summary>
        private void OnLoadMap(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnLoadMap;
            
            if (!scene.name.Contains("Multiplayer")) return;
            GameManager.Instance.SendOnPressRetryCallback(Restart);
            GameManager.Instance.SendOnPressMainMenu(OnGoToMainMenu);

            SetUpSnakes();
            
            GameManager.Instance.Retry();
        }
        
        /// <summary>
        /// Resets the snake quantity.
        /// </summary>
        private void Restart()
        {
            _alivePlayers = new List<SnakeManager>(_players);
            snakesQuantity = _alivePlayers.Count * 2;
        }
        
        /// <summary>
        /// Set up the snakes.
        /// </summary>
        private void SetUpSnakes()
        {
            Restart();
            
            foreach (SnakeManager player in _players)
            {
                GameObject ai = Instantiate(AIPrefab, transform);
                SnakeController aiController = ai.GetComponentInChildren<SnakeController>();
                aiController.SetColor(GetAIColor(player.Color));
                aiController.SetOnSnakeDieCallback(RemovePlayerFromDeath);
                
                player.ShowSnake();
                player.transform.position = Vector3.zero;
                player.GetSnakeController().SetOnSnakeDieCallback(RemovePlayerFromDeath);
                player.EnableSnakeController();
                player.GetSnakeController().SetColor(player.Color);
            }
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
