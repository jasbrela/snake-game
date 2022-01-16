using Snake;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Multiplayer
{
    public class SnakeInformation
    {
        public GameObject Card;
        public SnakeManager Manager;
        public PlayerInput Input;
        public Color Color;
        public readonly int ID;

        public SnakeInformation(int id)
        {
            ID = id;
            Manager = null;
            Input = null;
            Card = null;
            Color = Color.clear;
        }

        public void SetManager(SnakeManager manager)
        {
            Manager = manager;
            manager.Info = this;
        }
    }
}
