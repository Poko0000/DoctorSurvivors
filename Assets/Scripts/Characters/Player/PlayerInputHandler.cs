using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    private InputAction m_moveAction;
    public Vector2 MoveAmt => m_moveAction.ReadValue<Vector2>();
    void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }
    void Awake()
    {
        m_moveAction = InputSystem.actions.FindAction("Move");
    }

}
