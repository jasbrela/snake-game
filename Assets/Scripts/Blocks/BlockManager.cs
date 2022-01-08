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
        
        public PowerUp GetRandomBlockType()
        {
            double rollBuff = Math.Round(Random.Range(0f, 1f), 2);
        
            if (rollBuff > 0.5f)
            {
                return PowerUp.BatteringRam;
            }
            
            return PowerUp.EnginePower;
        }
        
    
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

        public Vector3 GetLastGeneratedBlockPosition()
        {   
            return _lastPosGenerated;
        }

        public void SendOnGeneratedRandomPositionCallback(OnGeneratedRandomPos onGeneratedRandomPos)
        {
            _onGeneratedRandomPos = onGeneratedRandomPos;
        }

        public void SendBoundsCollider(BoxCollider2D box)
        {
            _boundsCollider = box;
        }
    }
}
