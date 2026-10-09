using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance {get; private set;}

    [Header("玩家數值")]
    [SerializeField] float moveSpeed;
    [SerializeField] int playerHealth;
    [SerializeField] float baseLevelUpExp = 100;
    [SerializeField] float expGrowth = 1.1f;

    [Header("玩家UI")]
     [SerializeField] private GameObject backpackPanel;
    PlayerInputHandler m_playerInput;
    Rigidbody2D m_rigidbody;
    PlayerWeaponHandler m_weaponHandler;
    PlayerHealthHandler m_healthHandler;
    PlayerLevelHandler m_levelHandler;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        m_playerInput = GetComponent<PlayerInputHandler>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_weaponHandler = GetComponent<PlayerWeaponHandler>();
        m_healthHandler = GetComponent<PlayerHealthHandler>();
        m_levelHandler = GetComponent<PlayerLevelHandler>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_healthHandler.Initialize(playerHealth);
        m_levelHandler.Initialize(baseLevelUpExp, expGrowth);
        OnToggleBackpack();
    }

    // Update is called once per frame
    void Update()
    {

       if(m_playerInput.ToggleBackpack)
        {
            OnToggleBackpack();
        }

       if(m_playerInput.Pick)
        {
            WorldItemPickup.TryPickupNearest(transform.position);
        }
    }

    void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        m_rigidbody.MovePosition(m_rigidbody.position + m_playerInput.MoveAmt.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnToggleBackpack()
    {
        if (backpackPanel == null) return;
        backpackPanel.SetActive(!backpackPanel.activeSelf);
    }
}
