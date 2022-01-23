using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blocks
{
    public class BlockManager : MonoBehaviour
    {
        #region Singleton
        private static BlockManager _instance;

        public static BlockManager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                GameObject go = new GameObject("Block Manager");
                go.AddComponent<BlockManager>();
                return _instance;
            }
        }
        #endregion

        private int _playersQuantity = 1;
        private BoxCollider2D _boundsCollider;
        private Vector3 _lastPosGenerated;
        private readonly List<Vector3> blocksPosition = new List<Vector3>();
        public delegate void OnGeneratedRandomPos(Vector3 pos);
        private OnGeneratedRandomPos _onGeneratedRandomPos;
        public delegate void OnBlockIsPicked(int id);
        private OnBlockIsPicked _onBlockIsPicked;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        
        /// <summary>
        /// Used to send callbacks to be called when the Block Manager generates a new position.
        /// </summary>
        /// <param name="method">A method to be called</param>
        public void SendOnGeneratedRandomPositionCallback(OnGeneratedRandomPos method)
        {
            _onGeneratedRandomPos += method;
        }
       
        /// <summary>
        /// Used to send callbacks to be called when a Block is picked.
        /// </summary>
        /// <param name="method">A method to be called</param>
        public void SendOnBlockIsPickedCallback(OnBlockIsPicked method)
        {
            _onBlockIsPicked += method;
        }
        
        /// <summary>
        /// Get a random block type.
        /// </summary>
        /// <returns>A random block type</returns>
        public PowerUp GetRandomBlockType()
        {
            double rollBuff = Math.Round(Random.Range(0f, 1f), 2);
        
            if (rollBuff > 0.5f)
            {
                return PowerUp.BatteringRam;
            }
            
            return PowerUp.EnginePower;
        }
        
        /// <summary>
        /// Get a random position that exists within the bounds.
        /// </summary>
        /// <returns>A Vector3 of the random position</returns>
        public Vector3 GetRandomPosition()
        {
            Vector2 bounds = _boundsCollider.size / 2;
            Vector2 point = new Vector2(Mathf.Round(Random.Range(-bounds.x, bounds.x)),
                Mathf.Round(Random.Range(-bounds.y, bounds.y))) + _boundsCollider.offset;

            var pos = _boundsCollider.transform.TransformPoint(point);
            
            _lastPosGenerated = pos;
            _onGeneratedRandomPos?.Invoke(pos);

            return pos;
        }

        /// <summary>
        /// Get the last generated block position.
        /// </summary>
        /// <returns>A Vector3 of the last position generated</returns>
        public Vector3 GetLastGeneratedBlockPosition()
        {   
            return _lastPosGenerated;
        }

        /// <summary>
        /// Get the bound's collider.
        /// </summary>
        /// <returns>A BoxCollider2D of the bounds</returns>
        public BoxCollider2D GetBoundsCollider()
        {
            return _boundsCollider;
        }

        /// <summary>
        /// Used to send the BoxCollider2D of the bounds.
        /// </summary>
        /// <param name="box">A BoxCollider2D of the bounds</param>
        public void SendBoundsCollider(BoxCollider2D box)
        {
            _boundsCollider = box;
        }

        /// <summary>
        /// Updates the block's position.
        /// </summary>
        /// <param name="pos">The latest block's position</param>
        /// <param name="id">The block's id</param>
        public void NotifyBlockPositionGenerated(Vector3 pos, int id)
        {
            blocksPosition[id] = pos;
            _onBlockIsPicked?.Invoke(id);
        }
        
        /// <summary>
        /// Get this block's last generated position.
        /// </summary>
        /// <param name="id">This block's id</param>
        /// <returns>A Vector3 of the last position generated</returns>
        public Vector3 GetLastGeneratedBlockPosition(int id)
        {   
            return blocksPosition[id];
        }
        
        /// <summary>
        /// Sets the player quantity.
        /// </summary>
        /// <param name="quantity">The player's quantity</param>
        public void SetPlayersQuantity(int quantity)
        {
            _playersQuantity = quantity;
            blocksPosition.Clear();
            
            for (int i = 0; i < quantity; i++)
            {
                blocksPosition.Add(Vector3.zero);
            }
        }
    
        /// <summary>
        /// Gets the player quantity.
        /// </summary>
        /// <returns>How many human players are playing?</returns>
        public int GetPlayersQuantity()
        {
            return _playersQuantity;
        }
    }
}
