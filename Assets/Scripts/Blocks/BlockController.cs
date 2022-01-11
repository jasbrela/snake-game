using System;
using Enums;
using UnityEngine;

namespace Blocks
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boundsCollider;
        [SerializeField] private GameObject batteringRam;
        [SerializeField] private GameObject enginePower;
        public PowerUp Type { get; private set; }

        private void Awake()
        {
            BlockManager.Instance.SendBoundsCollider(boundsCollider);
            GameManager.Instance.SendOnGameStartsCallback(SetUpBlock);
        }

        /// <summary>
        /// Hide the block.
        /// </summary>
        private void ResetBlock()
        {
            if (batteringRam.activeInHierarchy)
            {
                batteringRam.SetActive(false);
            }
            
            if (enginePower.activeInHierarchy)
            {
                enginePower.SetActive(false);
            }
        }

        /// <summary>
        /// Show the right block regarding to its type.
        /// </summary>
        private void SetUpBlock()
        {
            ChangePosition();
            Type = BlockManager.Instance.GetRandomBlockType();
            
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

        /// <summary>
        /// Change the block position to a random one.
        /// </summary>
        private void ChangePosition()
        {
            transform.position = BlockManager.Instance.GetRandomPosition();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                ChangePosition();
            }
        }
        
        /// <summary>
        /// Respawn the block.
        /// </summary>
        public void Respawn()
        {
            ResetBlock();
            SetUpBlock();
        }

    }
}
