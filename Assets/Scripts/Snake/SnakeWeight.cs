using UnityEngine;

namespace Snake
{
    public class SnakeWeight : MonoBehaviour
    {
        [Tooltip("The smaller the value the faster")][SerializeField] private float speed;
        [SerializeField] private float weightPerBlock;
        [SerializeField] private float enginePowerBonus;
        private float _defaultSpeed;
    
        public float Speed => speed * 0.1f;
        private float WeightPerBlock => weightPerBlock * 0.1f;
        private float EnginePowerBonus => enginePowerBonus * 0.1f;

        private void Awake()
        {
            _defaultSpeed = speed;
        }

        /// <summary>
        /// Increase snake's speed whenever the snake pick up the Engine Power block.
        /// </summary>
        public void OnPickupEnginePowerBlock()
        {
            speed -= EnginePowerBonus;
        }

        /// <summary>
        /// Decrease snake's speed whenever the snake pick up any block.
        /// </summary>
        public void OnPickupAnyBlock()
        {
            speed += WeightPerBlock;
        }

        /// <summary>
        /// Increase snake's speed whenever the snake use its Battering Ram block.
        /// </summary>
        public void OnUseBatteringRam()
        {
            speed -= WeightPerBlock;
        }

        /// <summary>
        /// Reset the snake's speed to default.
        /// </summary>
        public void OnResetSnake()
        {
            speed = _defaultSpeed;
        }
    }
}
