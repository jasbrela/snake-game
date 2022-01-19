using UnityEngine;

namespace Multiplayer
{
    [CreateAssetMenu(fileName = "New Snake Preset", menuName = "Snake/Snake Preset", order = 0)]
    public class SnakePreset : ScriptableObject
    {
        public int batteringRamQuantity;
        public int enginePowerQuantity;
    }
}
