using System;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blocks
{
    public class BlockManager : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boundsCollider;
        [SerializeField] private GameObject batteringRam;
        [SerializeField] private GameObject energyEngine;
        
        private Vector3 _lastPosGenerated;
        public delegate void OnGeneratedRandomPosDelegate(Vector3 pos);
        private OnGeneratedRandomPosDelegate _onGeneratedRandomPosMethod;

        public PowerUp GetRandomBlockType()
        {
            double rollBuff = Math.Round(Random.Range(0f, 1f), 2);
        
            if (rollBuff > 0.5f)
            {
                batteringRam.SetActive(true);
                return PowerUp.BatteringRam;
            }

            energyEngine.SetActive(true);
            return PowerUp.EnginePower;
        }

        public void ResetBlock()
        {
            if (batteringRam.activeSelf)
            {
                batteringRam.SetActive(false);
            }
            if (energyEngine.activeSelf)
            {
                energyEngine.SetActive(false);
            }
        }
    
        public Vector3 GetRandomPosition(bool isBlockPosition)
        {
            Vector2 bounds = boundsCollider.size / 2;
            Vector2 point = new Vector2(Mathf.Round(Random.Range(-bounds.x, bounds.x)),
                Mathf.Round(Random.Range(-bounds.y, bounds.y))) + boundsCollider.offset;

            var pos = boundsCollider.transform.TransformPoint(point);
            
            if (!isBlockPosition) return pos;
            
            _lastPosGenerated = pos;
            _onGeneratedRandomPosMethod?.Invoke(pos);

            return pos;
        }

        public Vector3 GetLastGeneratedBlockPosition()
        {   
            return _lastPosGenerated;
        }

        public void SendOnGeneratedRandomPositionCallback(OnGeneratedRandomPosDelegate method)
        {
            _onGeneratedRandomPosMethod = method;
        }
    }
}
