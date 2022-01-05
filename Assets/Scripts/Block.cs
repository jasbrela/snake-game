using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BlockType
{
    EnergyEngine,
    BatteringRam
}
public class Block : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boundsCollider;
    [SerializeField] private GameObject batteringRam;
    [SerializeField] private GameObject energyEngine;

    public BlockType Type { get; private set; }

    void Start()
    {
        SetUpBlock();
    }

    private BlockType RandomizeType()
    {
        double rollBuff = Math.Round(Random.Range(0f, 1f), 2);
        
        if (rollBuff > 0.5f)
        {
            batteringRam.SetActive(true);
            return BlockType.BatteringRam;
        }

        energyEngine.SetActive(true);
        return BlockType.EnergyEngine;
    }

    private void SetUpBlock()
    {
        transform.position = GetRandomPosition();
        Type = RandomizeType();
    }

    public void RespawnBlock()
    {
        if (batteringRam.activeSelf)
        {
            batteringRam.SetActive(false);
        }
        if (energyEngine.activeSelf)
        {
            energyEngine.SetActive(false);
        }
        
        SetUpBlock();
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 bounds = boundsCollider.size / 2;
        Vector2 point = new Vector2(Mathf.Round(Random.Range(-bounds.x, bounds.x)),
            Mathf.Round(Random.Range(-bounds.y, bounds.y))) + boundsCollider.offset;

        return boundsCollider.transform.TransformPoint(point);
    }
}
