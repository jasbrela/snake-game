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

        /// <summary>
        /// Reset the power up quantities to zero and hide the Battering Ram Sprite
        /// </summary>
        public void ResetPowerUps()
        {
            if (batteringRam.activeInHierarchy) batteringRam.SetActive(false);
            _enginePowerQuantity = 0;
            _batteringRamQuantity = 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Block")) return;
            
            BlockController blockController = other.GetComponent<BlockController>();

            AddPowerUp(blockController.Type);
            blockController.Respawn();
        }
    
        /// <summary>
        /// Add one to the power-ups type quantity and show the Battering Ram sprite if necessary.
        /// </summary>
        /// <param name="type">Power Up Type</param>
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

        /// <summary>
        /// If the player has at least one Battering Ram block, it uses one.
        /// If it has only one, it also hides the Battering Ram sprite.
        /// </summary>
        /// <returns>A bool regarding the result of the use attempt.</returns>
        public bool TryUseBatteringRam()
        {
            if (_batteringRamQuantity <= 0) return false;
            
            _batteringRamQuantity--;
            if (batteringRam.activeSelf && _batteringRamQuantity == 0) batteringRam.SetActive(false);
            return true;
        }
    }
}
