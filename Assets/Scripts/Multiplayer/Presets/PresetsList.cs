using UnityEngine;

namespace Multiplayer
{
    [CreateAssetMenu(fileName = "New Presets List", menuName = "Snake/Presets List", order = 0)]
    public class PresetsList : ScriptableObject
    {
        public SnakePreset[] snakePresets;
    }
}