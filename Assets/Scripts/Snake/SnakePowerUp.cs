using Blocks;
using Enums;
using UnityEngine;

namespace Snake
{
    public class SnakePowerUp : MonoBehaviour
    {
        private int _enginePowerQuantity;
        private int _batteringRamQuantity;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Block")) return;
            
            Block block = other.GetComponent<Block>();

            AddPowerUp(block.Type);
            block.RespawnBlock();
        }
    
        private void AddPowerUp(PowerUp type)
        {
            switch (type)
            {
                case PowerUp.BatteringRam:
                    _batteringRamQuantity++;
                    break;
                case PowerUp.EnginePower:
                    _enginePowerQuantity++;
                    break;
            }
        }

        public bool TryUseBatteringRam()
        {
            if (_batteringRamQuantity <= 0) return false;
            _batteringRamQuantity--;
            return true;
        }
    }
}
