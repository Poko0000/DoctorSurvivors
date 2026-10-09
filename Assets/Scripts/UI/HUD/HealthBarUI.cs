using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 血條:訂閱 PlayerHealthHandler.OnHealthChanged,只負責顯示
public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;        // Image Type 要設成 Filled
    //[SerializeField] private TMP_Text valueText;     // 可不填,例如顯示 "80 / 100"

    private PlayerHealthHandler health;

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("[HealthBarUI] 找不到 PlayerController");
            return;
        }
        health = PlayerController.Instance.GetComponent<PlayerHealthHandler>();
        if (health == null)
        {
            Debug.LogError("[HealthBarUI] 玩家身上沒有 PlayerHealthHandler");
            return;
        }

        health.OnHealthChanged += Refresh;
        // 先拉一次目前數值(玩家可能比 HUD 早初始化,那次廣播已經錯過了)
        Refresh(health.CurrentHP, health.MaxHP);
    }

    void OnDestroy()
    {
        if (health != null) health.OnHealthChanged -= Refresh;
    }

    private void Refresh(int current, int max)
    {
        if (fillImage != null) fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
        //if (valueText != null) valueText.text = $"{current} / {max}";
    }
}
