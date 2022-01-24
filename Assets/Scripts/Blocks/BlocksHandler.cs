using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class BlocksHandler : MonoBehaviour
    {
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private BoxCollider2D boundsCollider;

        private readonly List<Block> blocks = new List<Block>();

        private void Awake()
        {
            BlockManager.Instance.SendBoundsCollider(boundsCollider);
            GameManager.Instance.SendOnGameStartsForTheFirstTimeCallback(SetUpBlocks);
            if (boundsCollider.enabled) boundsCollider.enabled = false;
        }

        /// <summary>
        /// Set up blocks for both multiplayer and singleplayer cases.
        /// </summary>
        private void SetUpBlocks()
        {
            if (GameManager.IsAMultiplayerGame())
            {
                for (int b = 0; b < BlockManager.Instance.GetPlayersQuantity(); b++)
                {
                    GameObject block = Instantiate(blockPrefab, transform);
                    blocks.Add(block.GetComponent<Block>());
                    blocks[b].SetBlockID(b);
                    blocks[b].SetCallbacks();
                    blocks[b].Respawn();
                }
            }
            else
            {
                GameObject blockObj = Instantiate(blockPrefab, transform);
                var block = blockObj.GetComponent<Block>();
                
                block.Respawn();
                block.SetCallbacks();
            }
        }
    }
}
