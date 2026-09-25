using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction m_moveAction;
    private InputAction m_toggleBackpackAction;
    public Vector2 MoveAmt => m_moveAction.ReadValue<Vector2>();
    public bool ToggleBackpack => m_toggleBackpackAction.WasPressedThisFrame();
    
    void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
        m_toggleBackpackAction = InputSystem.actions.FindAction("ToggleBackpack");
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
