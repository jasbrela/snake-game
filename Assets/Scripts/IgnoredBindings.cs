using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Multiplayer/IgnoredBindings", fileName = "New Ignored Bindings", order = 1)]
public class IgnoredBindings : ScriptableObject
{
    public InputAction bindings;
}
