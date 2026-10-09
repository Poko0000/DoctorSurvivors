using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 經驗條 + 等級:訂閱 PlayerLevelHandler 的 OnExpChanged / OnlevelUp
public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;     // Image Type 要設成 Filled
    [SerializeField] private TMP_Text levelText;  // 顯示 "Lv.3"

    private PlayerLevelHandler levelHandler;

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("[ExpBarUI] 找不到 PlayerController");
            return;
        }
        levelHandler = PlayerController.Instance.GetComponent<PlayerLevelHandler>();
        if (levelHandler == null)
        {
            Debug.LogError("[ExpBarUI] 玩家身上沒有 PlayerLevelHandler");
            return;
        }

        levelHandler.OnExpChanged += RefreshExp;
        levelHandler.OnlevelUp += RefreshLevel;
        RefreshExp(levelHandler.ExpRatio);
        RefreshLevel(levelHandler.Level);
    }

    void OnDestroy()
    {
        if (levelHandler == null) return;
        levelHandler.OnExpChanged -= RefreshExp;
        levelHandler.OnlevelUp -= RefreshLevel;
    }

    private void RefreshExp(float ratio)
    {
        if (fillImage != null) fillImage.fillAmount = Mathf.Clamp01(ratio);
    }

    private void RefreshLevel(int level)
    {
        if (levelText != null) levelText.text = "Lv." + level;
    }
}
