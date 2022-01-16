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
        [SerializeField] private TextMeshProUGUI powerEngineQuantity;
        
        [SerializeField] private PresetsList allPresets;
        
        [Header("Snake's Head")]
        [SerializeField] private SpriteRenderer headSprite;
        [SerializeField] private GameObject head;

        public event Action<int> OnClickDelete;

        private int index;
        [HideInInspector] public SnakePreset currentPreset;
        public SnakeInformation Info { get; set; }
        
        private void SetUpControls()
        {
            Info.Input.actions[InputActions.TurnLeft.ToString()].performed += _ => OnClickLeft();
            Info.Input.actions[InputActions.TurnRight.ToString()].performed += _ => OnClickRight();
        }

        public void ShowCard()
        {
            currentPreset = allPresets.snakePresets[0];
            headSprite.enabled = false;
            UpdateCard();
            card.SetActive(true);
            
            SetUpControls();
            Info.Input.actions.Enable();

            Debug.Log(Info.Input.actions.enabled);
        }

        private void UpdateCard()
        {
            UpdatePreset();
            
            nickname.text = $"P{Info.ID}";
            snakeHead.color = Info.Color;
        }
        
        private void UpdatePreset()
        {
            batteringRamQuantity.text = $"x{currentPreset.batteringRamQuantity}";
            powerEngineQuantity.text = $"x{currentPreset.powerEngineQuantity}";
        }

        public void ShowHead()
        {
            Info.Input.actions.Disable();
            
            card.SetActive(false);
            headSprite.enabled = true;
        }
        
        public void DeleteCard()
        {
            OnClickDelete?.Invoke(Info.ID - 1);
            Destroy(gameObject);
        }

        public GameObject GetHead()
        {
            return head;
        }

        private void OnClickRight()
        {
            index++;
            if (index >= allPresets.snakePresets.Length - 1) index = 0;
            currentPreset = allPresets.snakePresets[index];
            
            UpdatePreset();
        }

        private void OnClickLeft()
        {
            index--;
            if (index <= 0) index = allPresets.snakePresets.Length - 1;
            currentPreset = allPresets.snakePresets[index];
            
            UpdatePreset();
        }
    }
}
