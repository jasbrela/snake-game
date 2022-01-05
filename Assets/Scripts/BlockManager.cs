using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    #region Sigleton
    private static BlockManager _instance;

    public static BlockManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("Events Handler");
                go.AddComponent<BlockManager>();
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }
    #endregion

    [SerializeField] private Transform block;
    [SerializeField] private BoxCollider2D boundsCollider;
    [SerializeField] private GameObject batteringRam;
    [SerializeField] private GameObject energyEngine;
    
    public PowerUp GetRandomBlockType()
    {
        double rollBuff = Math.Round(Random.Range(0f, 1f), 2);
        
        if (rollBuff > 0.5f)
        {
            batteringRam.SetActive(true);
            return PowerUp.BatteringRam;
        }

        energyEngine.SetActive(true);
        return PowerUp.EnergyEngine;
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
    
    public Vector3 GetRandomPosition()
    {
        Vector2 bounds = boundsCollider.size / 2;
        Vector2 point = new Vector2(Mathf.Round(Random.Range(-bounds.x, bounds.x)),
            Mathf.Round(Random.Range(-bounds.y, bounds.y))) + boundsCollider.offset;

        return boundsCollider.transform.TransformPoint(point);
    }

    public Vector3 GetBlockPosition()
    {
        return block.position;
    }
}
