using Enums;
using UnityEngine;

public class AIHandler : MonoBehaviour
{
    [Header("Collision Predictor Points")]
    [SerializeField] private BoxCollider2D pointLeft;
    [SerializeField] private BoxCollider2D pointRight;
    [SerializeField] private BoxCollider2D pointFrontLeft;
    [SerializeField] private BoxCollider2D pointFrontRight;
    [SerializeField] private BoxCollider2D pointBackLeft;
    [SerializeField] private BoxCollider2D pointBackRight;
    
    [Space(10)][Header("Layers to Avoid")]
    [Tooltip("This layer allow the snake to use its battering ram blocks before dying")]
    [SerializeField] private LayerMask obstaclesLayer;
    
    [Tooltip("This layer will kill the snake instantly")]
    [SerializeField] private LayerMask wallsLayer;

    public bool IsLeftPointColliding()
    {
        return pointLeft.IsTouchingLayers(obstaclesLayer) || pointLeft.IsTouchingLayers(wallsLayer);
    }
    
    public bool IsRightPointColliding()
    {
        return pointRight.IsTouchingLayers(obstaclesLayer) || pointRight.IsTouchingLayers(wallsLayer);
    }

    public Directions GetTheFreerSide()
    {
        int leftSum = 0;
        int rightSum = 0;
            
        if (pointFrontLeft.IsTouchingLayers(obstaclesLayer)) leftSum++;
        if (pointFrontRight.IsTouchingLayers(obstaclesLayer)) leftSum++;
        if (pointBackLeft.IsTouchingLayers(obstaclesLayer)) rightSum++;
        if (pointBackRight.IsTouchingLayers(obstaclesLayer)) rightSum++;
        
        return leftSum > rightSum ? Directions.Left : Directions.Right;
    }
    
}
