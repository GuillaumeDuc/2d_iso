// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Mouse.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MouseInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MouseInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Mouse"",
    ""maps"": [
        {
            ""name"": ""Mouse"",
            ""id"": ""7ae89fbf-390d-450e-a77c-da67dbe6e646"",
            ""actions"": [
                {
                    ""name"": ""MouseClick"",
                    ""type"": ""Button"",
                    ""id"": ""3dd98cfb-e1cf-4f8f-aeb5-09da1b39847d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""3885c525-0839-4f90-88ae-6f0a4986e023"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3517bb9e-db72-4146-9b7f-d18f01b23ee7"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7f027fb-6b78-4f96-a869-5558970352f7"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_MouseClick = m_Mouse.FindAction("MouseClick", throwIfNotFound: true);
        m_Mouse_MousePosition = m_Mouse.FindAction("MousePosition", throwIfNotFound: true);
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

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_MouseClick;
    private readonly InputAction m_Mouse_MousePosition;
    public struct MouseActions
    {
        private @MouseInput m_Wrapper;
        public MouseActions(@MouseInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseClick => m_Wrapper.m_Mouse_MouseClick;
        public InputAction @MousePosition => m_Wrapper.m_Mouse_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @MouseClick.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
                @MouseClick.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
                @MouseClick.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouseClick;
                @MousePosition.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseClick.started += instance.OnMouseClick;
                @MouseClick.performed += instance.OnMouseClick;
                @MouseClick.canceled += instance.OnMouseClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    public interface IMouseActions
    {
        void OnMouseClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
