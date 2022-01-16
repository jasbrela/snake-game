using Enums;
using UnityEngine;

namespace Snake
{
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

        /// <summary>
        /// Used to check if the left point is touching with an obstacle or a wall.
        /// </summary>
        /// <returns>A bool regarding the collision.</returns>
        public bool IsLeftPointColliding()
        {
            return pointLeft.IsTouchingLayers(obstaclesLayer) || pointLeft.IsTouchingLayers(wallsLayer);
        }
    
        /// <summary>
        /// Used to check if the right point is touching with an obstacle or a wall.
        /// </summary>
        /// <returns>A bool regarding the collision.</returns>
        public bool IsRightPointColliding()
        {
            return pointRight.IsTouchingLayers(obstaclesLayer) || pointRight.IsTouchingLayers(wallsLayer);
        }

        /// <summary>
        /// Checks the snake's corner points to return the freer side.
        /// </summary>
        /// <returns>Return the snake's freer side.</returns>
        public Directions GetTheFreerSide()
        {
            int leftSum = 0;
            int rightSum = 0;

            // Whenever it is touching a killing layer, add one to the sum.
            if (pointFrontLeft.IsTouchingLayers(obstaclesLayer) ||
                pointFrontLeft.IsTouchingLayers(wallsLayer)) leftSum++;

            if (pointFrontRight.IsTouchingLayers(obstaclesLayer) ||
                pointFrontLeft.IsTouchingLayers(wallsLayer)) leftSum++;

            if (pointBackLeft.IsTouchingLayers(obstaclesLayer) ||
                pointBackLeft.IsTouchingLayers(wallsLayer)) rightSum++;
        
            if (pointBackRight.IsTouchingLayers(obstaclesLayer) ||
                pointBackRight.IsTouchingLayers(wallsLayer)) rightSum++;
        
            // The lesser the sum, the freer the side.
            return leftSum < rightSum ? Directions.Left : Directions.Right;
        }
    
    }
}
