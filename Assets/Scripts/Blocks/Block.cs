using Enums;
using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] BlockManager blockManager;
        [SerializeField] private LayerMask obstacleLayer;
        public PowerUp Type { get; private set; }
        private BoxCollider2D _collider2D;

        void Start()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            SetUpBlock();
        }

        private void SetUpBlock()
        {
            ChangePosition();
            Type = blockManager.GetRandomBlockType();
        }

        private void ChangePosition()
        {
            // TODO: Blocks are spawning inside snake   
            transform.position = blockManager.GetRandomPosition(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                ChangePosition();
            }
        }

        public void RespawnBlock()
        {
            blockManager.ResetBlock();
            SetUpBlock();
        }

    }
}
