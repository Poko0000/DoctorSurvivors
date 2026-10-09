using System.Collections.Generic;
using UnityEngine;

// 背包面板的開關控制(唯一可以開關 BackpackPanel 的地方)
//   - 玩家按鍵:PlayerController 呼叫 Toggle()
//   - 升級三選一:監聽 LevelUpRewardSystem 的事件,自動打開;選完後恢復原本的開關狀態
// 掛在一個「一直啟用」的物件上,不要掛在 BackpackPanel 自己身上(理由跟 ResultUI 一樣)
public class BackpackPanelController : MonoBehaviour
{
    [SerializeField] private GameObject backpackPanel;

    private LevelUpRewardSystem rewardSystem;

    private bool isForcedOpen;          // 目前是否被升級流程強制打開
    private bool wasOpenBeforeForce;    // 被強制打開之前,玩家自己是開著還是關著

    public bool IsOpen => backpackPanel != null && backpackPanel.activeSelf;

    void Start()
    {
        if (backpackPanel == null)
        {
            Debug.LogError("[BackpackPanel] 沒有指定 backpackPanel,背包無法開關。");
        }

        // 遊戲開始時先關閉背包(取代原本 PlayerController.Start 裡的 OnToggleBackpack)
        SetOpen(false);

        // 沒有升級系統時,背包仍然可以手動開關,所以只警告不中斷
        rewardSystem = LevelUpRewardSystem.Instance;
        if (rewardSystem == null)
        {
            Debug.LogWarning("[BackpackPanel] 找不到 LevelUpRewardSystem,升級時不會自動打開背包。");
            return;
        }

        rewardSystem.OnChoicesReady += HandleChoicesReady;
        rewardSystem.OnAllChoicesDone += HandleAllChoicesDone;
    }

    void OnDestroy()
    {
        if (rewardSystem != null)
        {
            rewardSystem.OnChoicesReady -= HandleChoicesReady;
            rewardSystem.OnAllChoicesDone -= HandleAllChoicesDone;
        }
    }

    // ===== 給 PlayerController 的按鍵呼叫 =====

    public void Toggle()
    {
        // 升級選擇期間背包必須保持開啟,不讓玩家關掉
        // (遊戲暫停時 Update 仍會執行,所以按鍵還是會進來,要在這裡擋)
        if (isForcedOpen) return;

        SetOpen(!IsOpen);
    }

    // ===== 升級事件 =====

    // 參數型別要跟事件一致(Action<List<ItemData>>),這裡用不到 choices,但簽名必須相符
    private void HandleChoicesReady(List<ItemData> choices)
    {
        // 連續升級時 OnChoicesReady 會觸發多次,只有第一次要記住原本狀態
        if (isForcedOpen) return;

        wasOpenBeforeForce = IsOpen;
        isForcedOpen = true;
        SetOpen(true);
    }

    private void HandleAllChoicesDone()
    {
        if (!isForcedOpen) return;

        isForcedOpen = false;
        SetOpen(wasOpenBeforeForce);   // 恢復成玩家原本的狀態
    }

    // ===== 內部 =====

    private void SetOpen(bool open)
    {
        if (backpackPanel != null)
        {
            backpackPanel.SetActive(open);
        }
    }
}
