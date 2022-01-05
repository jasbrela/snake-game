// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""82f1beef-967d-4aff-844b-ed23d4f7a60c"",
            ""actions"": [
                {
                    ""name"": ""TurnLeft"",
                    ""type"": ""Value"",
                    ""id"": ""5717636d-ba1a-4732-abac-a66629a7ba94"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TurnRight"",
                    ""type"": ""Button"",
                    ""id"": ""a1f577b5-e3a0-4976-be0a-5931cccffe47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TurnUp"",
                    ""type"": ""Button"",
                    ""id"": ""bcd6e778-cd13-4eff-95df-d44143a6e4b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TurnDown"",
                    ""type"": ""Button"",
                    ""id"": ""f3a7f9a4-bfd9-4f97-a1e8-8d6a3fd220b5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c95c9589-2814-4bc6-898d-441062e2764e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""TurnLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8e66dec9-e018-47d8-8a19-76b54d1c87b3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""TurnRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55308f77-c794-4763-8a18-9da7b7739203"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""TurnUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5af0b2c0-7529-4f80-a3b2-0f3c2dcbae9b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""TurnDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_TurnLeft = m_Player.FindAction("TurnLeft", throwIfNotFound: true);
        m_Player_TurnRight = m_Player.FindAction("TurnRight", throwIfNotFound: true);
        m_Player_TurnUp = m_Player.FindAction("TurnUp", throwIfNotFound: true);
        m_Player_TurnDown = m_Player.FindAction("TurnDown", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_TurnLeft;
    private readonly InputAction m_Player_TurnRight;
    private readonly InputAction m_Player_TurnUp;
    private readonly InputAction m_Player_TurnDown;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @TurnLeft => m_Wrapper.m_Player_TurnLeft;
        public InputAction @TurnRight => m_Wrapper.m_Player_TurnRight;
        public InputAction @TurnUp => m_Wrapper.m_Player_TurnUp;
        public InputAction @TurnDown => m_Wrapper.m_Player_TurnDown;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @TurnLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnLeft;
                @TurnLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnLeft;
                @TurnLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnLeft;
                @TurnRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnRight;
                @TurnRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnRight;
                @TurnRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnRight;
                @TurnUp.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnUp;
                @TurnUp.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnUp;
                @TurnUp.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnUp;
                @TurnDown.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnDown;
                @TurnDown.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnDown;
                @TurnDown.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTurnDown;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @TurnLeft.started += instance.OnTurnLeft;
                @TurnLeft.performed += instance.OnTurnLeft;
                @TurnLeft.canceled += instance.OnTurnLeft;
                @TurnRight.started += instance.OnTurnRight;
                @TurnRight.performed += instance.OnTurnRight;
                @TurnRight.canceled += instance.OnTurnRight;
                @TurnUp.started += instance.OnTurnUp;
                @TurnUp.performed += instance.OnTurnUp;
                @TurnUp.canceled += instance.OnTurnUp;
                @TurnDown.started += instance.OnTurnDown;
                @TurnDown.performed += instance.OnTurnDown;
                @TurnDown.canceled += instance.OnTurnDown;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnTurnLeft(InputAction.CallbackContext context);
        void OnTurnRight(InputAction.CallbackContext context);
        void OnTurnUp(InputAction.CallbackContext context);
        void OnTurnDown(InputAction.CallbackContext context);
    }
}
