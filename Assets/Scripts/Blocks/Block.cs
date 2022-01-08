using Enums;
using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] BlockManager blockManager;
        public PowerUp Type { get; private set; }

        void Start()
        {
            GetComponent<BoxCollider2D>();
            SetUpBlock();
        }

        private void SetUpBlock()
        {
            ChangePosition();
            Type = blockManager.GetRandomBlockType();
        }

        private void ChangePosition()
        {
            // TODO: Blocks can spawn inside snake
            transform.position = blockManager.GetRandomPosition();
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
