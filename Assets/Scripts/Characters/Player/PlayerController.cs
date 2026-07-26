using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    PlayerInputHandler m_playerInput;
    Rigidbody2D m_rigidbody;
    PlayerWeaponHandler m_weaponHandler;
    [SerializeField] WeaponData weapon;

    void Awake()
    {
        m_playerInput = GetComponent<PlayerInputHandler>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_weaponHandler = GetComponent<PlayerWeaponHandler>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_weaponHandler.AddWeapon(weapon);
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
