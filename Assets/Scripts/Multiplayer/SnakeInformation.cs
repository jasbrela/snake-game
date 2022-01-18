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
        public int ID;
        
        public SnakeInformation(int id)
        {
            ID = id;
            Manager = null;
            Input = null;
            Card = null;
            Color = Color.clear;
        }

        /// <summary>
        /// Create a link between the manager and this SnakeInformation.
        /// </summary>
        /// <param name="manager">a SnakeManager to link</param>
        public void SetManager(SnakeManager manager)
        {
            Manager = manager;
            manager.Info = this;
        }
    }
}
