using System;
using Enums;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Snake
{
    public class SnakeManager : MonoBehaviour
    {
        [Header("Snake")]
        [SerializeField] public PlayerInput input;
        
        [Header("Snake's Card")]
        [SerializeField] private GameObject card;
        [SerializeField] private Image snakeHead;
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI batteringRamQuantity;
        [SerializeField] private TextMeshProUGUI enginePowerQuantity;
        [SerializeField] private PresetsList allPresets;

        [Header("Snake's Head")]
        [SerializeField] private SnakeController controller;
        [SerializeField] private SpriteRenderer headSprite;

        public int ID { get; set; }
        public Color Color { get; set; }
        
        public event Action<int> OnClickDelete;

        private int index;
        [HideInInspector] public SnakePreset currentPreset;

        /// <summary>
        /// Set up the controls for human players.
        /// </summary>
        private void SetUpControls()
        {
            input.actions[InputActions.TurnLeft.ToString()].performed += _ => OnClickLeft();
            input.actions[InputActions.TurnRight.ToString()].performed += _ => OnClickRight();
        }

        /// <summary>
        /// Used to change the player's ID.
        /// </summary>
        /// <param name="newID">The new ID.</param>
        public void UpdateID(int newID)
        {
            ID = newID;
            UpdateCard();
        }
        
        /// <summary>
        /// Hide snake's head and show card. Used in Multiplayer Preparation.
        /// </summary>
        public void ShowCard()
        {
            currentPreset = allPresets.snakePresets[0];
            headSprite.enabled = false;
            UpdateCard();
            card.SetActive(true);
            
            SetUpControls();
            input.actions.Enable();
        }

        /// <summary>
        /// Update Card's info.
        /// </summary>
        private void UpdateCard()
        {
            UpdatePreset();
            
            nickname.text = $"P{ID}";
            snakeHead.color = Color;
        }
        
        /// <summary>
        /// Update with current preset values.
        /// </summary>
        private void UpdatePreset()
        {
            batteringRamQuantity.text = $"x{currentPreset.batteringRamQuantity}";
            enginePowerQuantity.text = $"x{currentPreset.enginePowerQuantity}";
        }

        /// <summary>
        /// Hide the card and show head.
        /// </summary>
        public void ShowHead()
        {
            card.SetActive(false);
            headSprite.enabled = true;
        }

        /// <summary>
        /// Show the snake
        /// </summary>
        public void ShowSnake()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Remove the player.
        /// </summary>
        public void RemovePlayer()
        {
            OnClickDelete?.Invoke(ID - 1);
            Destroy(gameObject);
        }

        /// <summary>
        /// Get this Snake's SnakeController.
        /// </summary>
        /// <returns>This Snake's SnakeController</returns>
        public ref SnakeController GetSnakeController()
        {
            return ref controller;
        }
        
        /// <summary>
        /// Enable this Snake's SnakeController.
        /// </summary>
        public void EnableSnakeController()
        {
            controller.enabled = true;
        }

        /// <summary>
        /// Change the Current Preset to the previous one. If it's the first, then change to the last preset available.
        /// </summary>
        private void OnClickRight()
        {
            if (SceneManager.GetActiveScene().name != Scenes.Preparation.ToString()) return;

            index++;
            if (index >= allPresets.snakePresets.Length - 1) index = 0;
            currentPreset = allPresets.snakePresets[index];
            
            UpdatePreset();
        }

        /// <summary>
        /// Change the Current Preset to the next one. If it's the last, then change to thr first preset available.
        /// </summary>
        private void OnClickLeft()
        {
            if (SceneManager.GetActiveScene().name != Scenes.Preparation.ToString()) return;
            
            index--;
            if (index <= 0) index = allPresets.snakePresets.Length - 1;
            currentPreset = allPresets.snakePresets[index];

            UpdatePreset();
        }
    }
}
