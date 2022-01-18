using System;
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

        private BoxCollider2D _boundsCollider;
        private Vector3 _lastPosGenerated;
        public delegate void OnGeneratedRandomPos(Vector3 pos);
        private OnGeneratedRandomPos _onGeneratedRandomPos;
        
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
        /// Used to send callbacks to be called when the Block Manager generates a new position.
        /// </summary>
        /// <param name="method">A method to be called</param>
        public void SendOnGeneratedRandomPositionCallback(OnGeneratedRandomPos method)
        {
            _onGeneratedRandomPos = method;
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
    }
}
