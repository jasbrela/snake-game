using Enums;
using UnityEngine;

namespace Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] BlockManager _blockManager;
        public PowerUp Type { get; private set; }

        void Start()
        {
            SetUpBlock();
        }

        private void SetUpBlock()
        {
            transform.position = _blockManager.GetRandomPosition();
            Type = _blockManager.GetRandomBlockType();
        }

        public void RespawnBlock()
        {
            _blockManager.ResetBlock();
            SetUpBlock();
        }

    }
}
