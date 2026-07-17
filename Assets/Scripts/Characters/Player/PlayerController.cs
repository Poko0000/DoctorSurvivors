using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    PlayerInputHandler m_playerInput;
    Rigidbody2D m_rigidbody;

    void Awake()
    {
        m_playerInput = GetComponent<PlayerInputHandler>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        m_rigidbody.MovePosition(m_rigidbody.position + m_playerInput.MoveAmt.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
