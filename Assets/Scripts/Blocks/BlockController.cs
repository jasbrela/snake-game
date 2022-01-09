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

        void Start()
        {
            BlockManager.Instance.SendBoundsCollider(boundsCollider);
            SetUpBlock();
        }

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

        private void ChangePosition()
        {
            // TODO: Blocks can spawn inside snake
            transform.position = BlockManager.Instance.GetRandomPosition();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle"))
            {
                ChangePosition();
            }
        }

        public void Respawn()
        {
            ResetBlock();
            SetUpBlock();
        }

    }
}
