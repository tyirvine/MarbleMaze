// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Z - Board/NewInputControls/BoardActionControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @BoardActionControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @BoardActionControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BoardActionControls"",
    ""maps"": [
        {
            ""name"": ""KeyboardControls"",
            ""id"": ""57681eb8-3f40-4162-bc28-22a43c84a408"",
            ""actions"": [
                {
                    ""name"": ""Tilt_X"",
                    ""type"": ""Button"",
                    ""id"": ""a61dd4a7-cab0-4160-a8ac-4ce68c20f22a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tilt_Z"",
                    ""type"": ""Button"",
                    ""id"": ""d67e31c9-cdb6-4f68-aa4f-9e7cf8f2d2c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""TiltX"",
                    ""id"": ""6b2eac2b-c3bf-487a-b630-312aab5a9e16"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_X"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ee971225-9c9a-43fc-877a-3dcf89813b3d"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""e9d638da-6e85-4184-bb3c-383daccebb4b"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""TiltZ"",
                    ""id"": ""c8cd389f-95b2-4908-b00b-55f3f0a1d660"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_Z"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""352e0800-b8fc-461d-92b3-933503172823"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_Z"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""00ab0ab6-a462-4564-8704-965dfca2fb8d"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tilt_Z"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // KeyboardControls
        m_KeyboardControls = asset.FindActionMap("KeyboardControls", throwIfNotFound: true);
        m_KeyboardControls_Tilt_X = m_KeyboardControls.FindAction("Tilt_X", throwIfNotFound: true);
        m_KeyboardControls_Tilt_Z = m_KeyboardControls.FindAction("Tilt_Z", throwIfNotFound: true);
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

    // KeyboardControls
    private readonly InputActionMap m_KeyboardControls;
    private IKeyboardControlsActions m_KeyboardControlsActionsCallbackInterface;
    private readonly InputAction m_KeyboardControls_Tilt_X;
    private readonly InputAction m_KeyboardControls_Tilt_Z;
    public struct KeyboardControlsActions
    {
        private @BoardActionControls m_Wrapper;
        public KeyboardControlsActions(@BoardActionControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Tilt_X => m_Wrapper.m_KeyboardControls_Tilt_X;
        public InputAction @Tilt_Z => m_Wrapper.m_KeyboardControls_Tilt_Z;
        public InputActionMap Get() { return m_Wrapper.m_KeyboardControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardControlsActions set) { return set.Get(); }
        public void SetCallbacks(IKeyboardControlsActions instance)
        {
            if (m_Wrapper.m_KeyboardControlsActionsCallbackInterface != null)
            {
                @Tilt_X.started -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_X;
                @Tilt_X.performed -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_X;
                @Tilt_X.canceled -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_X;
                @Tilt_Z.started -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_Z;
                @Tilt_Z.performed -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_Z;
                @Tilt_Z.canceled -= m_Wrapper.m_KeyboardControlsActionsCallbackInterface.OnTilt_Z;
            }
            m_Wrapper.m_KeyboardControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Tilt_X.started += instance.OnTilt_X;
                @Tilt_X.performed += instance.OnTilt_X;
                @Tilt_X.canceled += instance.OnTilt_X;
                @Tilt_Z.started += instance.OnTilt_Z;
                @Tilt_Z.performed += instance.OnTilt_Z;
                @Tilt_Z.canceled += instance.OnTilt_Z;
            }
        }
    }
    public KeyboardControlsActions @KeyboardControls => new KeyboardControlsActions(this);
    public interface IKeyboardControlsActions
    {
        void OnTilt_X(InputAction.CallbackContext context);
        void OnTilt_Z(InputAction.CallbackContext context);
    }
}
