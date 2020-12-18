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
            ""name"": ""Board"",
            ""id"": ""57681eb8-3f40-4162-bc28-22a43c84a408"",
            ""actions"": [
                {
                    ""name"": ""Tilt_X"",
                    ""type"": ""Button"",
                    ""id"": ""a61dd4a7-cab0-4160-a8ac-4ce68c20f22a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""Clamp(min=-1,max=1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tilt_Z"",
                    ""type"": ""Button"",
                    ""id"": ""d67e31c9-cdb6-4f68-aa4f-9e7cf8f2d2c4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""Clamp(min=-1,max=1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""05d954e1-fcee-41ef-ac02-363c432496e5"",
                    ""expectedControlType"": ""Vector2"",
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
                },
                {
                    ""name"": ""Direction"",
                    ""id"": ""5e386443-9d56-4f3f-8076-aad9c03af56f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Up"",
                    ""id"": ""8701fa96-e6c6-482d-94f5-6a323980e749"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Down"",
                    ""id"": ""adfbabbf-5e69-4958-9c98-95b6c6b544c6"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""4097025d-e4fd-401d-9743-4acb48ec3104"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""f2e5b480-7d22-4097-bfff-ce3ee9002830"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
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
        // Board
        m_Board = asset.FindActionMap("Board", throwIfNotFound: true);
        m_Board_Tilt_X = m_Board.FindAction("Tilt_X", throwIfNotFound: true);
        m_Board_Tilt_Z = m_Board.FindAction("Tilt_Z", throwIfNotFound: true);
        m_Board_Move = m_Board.FindAction("Move", throwIfNotFound: true);
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

    // Board
    private readonly InputActionMap m_Board;
    private IBoardActions m_BoardActionsCallbackInterface;
    private readonly InputAction m_Board_Tilt_X;
    private readonly InputAction m_Board_Tilt_Z;
    private readonly InputAction m_Board_Move;
    public struct BoardActions
    {
        private @BoardActionControls m_Wrapper;
        public BoardActions(@BoardActionControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Tilt_X => m_Wrapper.m_Board_Tilt_X;
        public InputAction @Tilt_Z => m_Wrapper.m_Board_Tilt_Z;
        public InputAction @Move => m_Wrapper.m_Board_Move;
        public InputActionMap Get() { return m_Wrapper.m_Board; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BoardActions set) { return set.Get(); }
        public void SetCallbacks(IBoardActions instance)
        {
            if (m_Wrapper.m_BoardActionsCallbackInterface != null)
            {
                @Tilt_X.started -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_X;
                @Tilt_X.performed -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_X;
                @Tilt_X.canceled -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_X;
                @Tilt_Z.started -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_Z;
                @Tilt_Z.performed -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_Z;
                @Tilt_Z.canceled -= m_Wrapper.m_BoardActionsCallbackInterface.OnTilt_Z;
                @Move.started -= m_Wrapper.m_BoardActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_BoardActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_BoardActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_BoardActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Tilt_X.started += instance.OnTilt_X;
                @Tilt_X.performed += instance.OnTilt_X;
                @Tilt_X.canceled += instance.OnTilt_X;
                @Tilt_Z.started += instance.OnTilt_Z;
                @Tilt_Z.performed += instance.OnTilt_Z;
                @Tilt_Z.canceled += instance.OnTilt_Z;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public BoardActions @Board => new BoardActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IBoardActions
    {
        void OnTilt_X(InputAction.CallbackContext context);
        void OnTilt_Z(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
}
