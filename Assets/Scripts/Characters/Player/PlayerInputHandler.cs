using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction m_moveAction;
    private InputAction m_toggleBackpackAction;
    private InputAction m_pickAction;
    public Vector2 MoveAmt => m_moveAction.ReadValue<Vector2>();
    public bool ToggleBackpack => m_toggleBackpackAction.WasPressedThisFrame();
    public bool Pick => m_pickAction != null && m_pickAction.WasPressedThisFrame();
    
    void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_toggleBackpackAction = InputSystem.actions.FindAction("ToggleBackpack");
        m_pickAction = InputSystem.actions.FindAction("Pick");
    }
    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

}
