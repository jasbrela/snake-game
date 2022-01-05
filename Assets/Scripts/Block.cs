using UnityEngine;

public class Block : MonoBehaviour
{
    public PowerUp Type { get; private set; }

    void Start()
    {
        SetUpBlock();
    }

    private void SetUpBlock()
    {
        transform.position = BlockManager.Instance.GetRandomPosition();
        Type = BlockManager.Instance.GetRandomBlockType();
    }

    public void RespawnBlock()
    {
        BlockManager.Instance.ResetBlock();
        SetUpBlock();
    }


}
