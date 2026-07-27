using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] int playerHealth;
    PlayerInputHandler m_playerInput;
    Rigidbody2D m_rigidbody;
    PlayerWeaponHandler m_weaponHandler;
    PlayerHealthHandler m_healthHandler;
    [SerializeField] WeaponData weapon;

    void Awake()
    {
        m_playerInput = GetComponent<PlayerInputHandler>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_weaponHandler = GetComponent<PlayerWeaponHandler>();
        m_healthHandler = GetComponent<PlayerHealthHandler>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_weaponHandler.AddWeapon(weapon);
        m_healthHandler.Initialize(playerHealth);
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
