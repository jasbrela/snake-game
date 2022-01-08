using System;
using Blocks;
using Enums;
using UnityEngine;

namespace Snake
{
    public class SnakePowerUp : MonoBehaviour
    {
        [SerializeField] GameObject batteringRam;
        private int _enginePowerQuantity;
        private int _batteringRamQuantity;

        public void ResetPowerUps()
        {
            if (batteringRam.activeInHierarchy) batteringRam.SetActive(false);
            _enginePowerQuantity = 0;
            _batteringRamQuantity = 0;
        }

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
                    if (!batteringRam.activeSelf) batteringRam.SetActive(true);
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
            if (batteringRam.activeSelf && _batteringRamQuantity == 0) batteringRam.SetActive(false);
            return true;
        }
    }
}
