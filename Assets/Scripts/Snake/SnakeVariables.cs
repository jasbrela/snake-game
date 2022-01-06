using System;
using UnityEngine;

namespace Snake
{
    public class SnakeVariables : MonoBehaviour
    {
        [Tooltip("The smaller the value the faster")][SerializeField] private float speed;
        [SerializeField] private float weightPerBlock;
        [SerializeField] private float energySpeedBonus;
        private float _defaultSpeed;
    
        public float Speed => speed * 0.1f;
        private float WeightPerBlock => weightPerBlock * 0.1f;
        private float EnergySpeedBonus => energySpeedBonus * 0.1f;

        private void Awake()
        {
            _defaultSpeed = speed;
        }

        public void OnPickupEnergyEngineBlock()
        {
            speed -= EnergySpeedBonus;
        }

        public void OnPickupAnyBlock()
        {
            speed += WeightPerBlock;
        }

        public void OnUseBatteringRam()
        {
            speed -= WeightPerBlock;
        }

        public void OnResetSnake()
        {
            speed = _defaultSpeed;
        }
    }
}
