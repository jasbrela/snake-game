using System;
using Enums;
using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Snake
{
    public class SnakeManager : MonoBehaviour
    {
        [Header("Snake's Card")]
        [SerializeField] private GameObject card;
        [SerializeField] private Image snakeHead;
        [SerializeField] private TextMeshProUGUI nickname;
        [SerializeField] private TextMeshProUGUI batteringRamQuantity;
        [SerializeField] private TextMeshProUGUI enginePowerQuantity;
        
        [SerializeField] private PresetsList allPresets;
        
        [Header("Snake's Head")]
        [SerializeField] private SpriteRenderer headSprite;
        [SerializeField] private GameObject head;

        public event Action<int> OnClickDelete;

        private int index;
        [HideInInspector] public SnakePreset currentPreset;
        public SnakeInformation Info { get; set; }
        
        /// <summary>
        /// Set up the controls for human players.
        /// </summary>
        private void SetUpControls()
        {
            Info.Input.actions[InputActions.TurnLeft.ToString()].performed += _ => OnClickLeft();
            Info.Input.actions[InputActions.TurnRight.ToString()].performed += _ => OnClickRight();
        }

        /// <summary>
        /// Used to change the player's ID.
        /// </summary>
        /// <param name="newID">The new ID.</param>
        public void UpdateID(int newID)
        {
            Info.ID = newID;
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
            Info.Input.actions.Enable();
        }

        /// <summary>
        /// Update Card's info.
        /// </summary>
        private void UpdateCard()
        {
            UpdatePreset();
            
            nickname.text = $"P{Info.ID}";
            snakeHead.color = Info.Color;
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
            Info.Input.actions.Disable();
            
            card.SetActive(false);
            headSprite.enabled = true;
        }
        
        /// <summary>
        /// Remove the player.
        /// </summary>
        public void RemovePlayer()
        {
            OnClickDelete?.Invoke(Info.ID - 1);
            Destroy(gameObject);
        }

        /// <summary>
        /// Get this Snake's head.
        /// </summary>
        /// <returns>A GameObject of a Snake's Head</returns>
        public GameObject GetHead()
        {
            return head;
        }

        /// <summary>
        /// Change the Current Preset to the previous one. If it's the first, then change to the last preset available.
        /// </summary>
        private void OnClickRight()
        {
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
            index--;
            if (index <= 0) index = allPresets.snakePresets.Length - 1;
            currentPreset = allPresets.snakePresets[index];
            
            UpdatePreset();
        }
    }
}
