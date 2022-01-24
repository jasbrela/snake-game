using Enums;
using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private GameObject batteringRam;
        [SerializeField] private GameObject enginePower;

        private int id;
        public PowerUp Type { get; private set; }
        
        /// <summary>
        /// Sets the block's id
        /// </summary>
        /// <param name="blockID">A int to be the block's id</param>
        public void SetBlockID(int blockID)
        {
            id = blockID;
        }

        /// <summary>
        /// Send a reset method to be called when player presses Retry.
        /// </summary>
        public void SetCallbacks()
        {
            GameManager.Instance.SendOnPressRetryCallback(HideBlock);
            GameManager.Instance.SendOnGameStartsCallback(Respawn);
        }

        /// <summary>
        /// Respawn the block.
        /// </summary>
        public void Respawn()
        {
            HideBlock();
            ChangePosition();
            RandomizeBlockType();
        }
    
        /// <summary>
        /// Hide the block.
        /// </summary>
        private void HideBlock()
        {
            batteringRam.SetActive(false);
            enginePower.SetActive(false);
        }
    
        /// <summary>
        /// Change the block position to a random one.
        /// </summary>
        private void ChangePosition()
        {
            transform.position = BlockManager.Instance.GetRandomPosition();
            if (GameManager.IsAMultiplayerGame()) BlockManager.Instance.NotifyBlockPositionGenerated(transform.position, id);
        }

        /// <summary>
        /// Sets the block type.
        /// </summary>
        private void RandomizeBlockType()
        {
            Type = BlockManager.Instance.GetRandomBlockType();;

            switch (Type)
            {
                case PowerUp.BatteringRam:
                    batteringRam.SetActive(true);
                    break;
                case PowerUp.EnginePower:
                    enginePower.SetActive(true);
                    break;
            }
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                Respawn();
            }
        }
    }
}
