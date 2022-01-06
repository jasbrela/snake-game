using Blocks;
using Enums;
using TMPro;
using UnityEngine;

namespace Snake
{
    public class SnakePowerUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private int _energyEngineQuantity;
        private int _batteringRamQuantity;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Block"))
            {
                Block block = other.GetComponent<Block>();

                AddPowerUp(block.Type);
                block.RespawnBlock();
            }
        }
    
        private void AddPowerUp(PowerUp type)
        {
            if (type == PowerUp.BatteringRam) { _batteringRamQuantity++; OnChangeBatteringRamQuantity();}
            if (type == PowerUp.EnergyEngine) _energyEngineQuantity++;
        }

        public bool TryUseBatteringRam()
        {
            if (_batteringRamQuantity <= 0) return false;
            _batteringRamQuantity--;
            OnChangeBatteringRamQuantity();
            return true;
        }

        private void OnChangeBatteringRamQuantity()
        {
            if (text != null) text.text = $"x{_batteringRamQuantity}";
        }

    }
}
