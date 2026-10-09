using UnityEngine;

// HUD 的總開關:只在 Playing 時顯示,結算畫面出現時隱藏
// 各個小元件(血條、經驗條、計時、擊殺數)各自訂閱自己的資料來源,這裡不管內容
public class BattleHUD : MonoBehaviour
{
    [SerializeField] private GameObject root;   // HUD 的所有元件都放在這底下

    private GameFlowController flow;

    void Start()
    {
        flow = GameFlowController.Instance;
        if (flow == null)
        {
            Debug.LogError("[BattleHUD] 找不到 GameFlowController");
            return;
        }

        flow.OnStateChanged += HandleStateChanged;
        // 訂閱前狀態可能已經切過了,先手動套用一次目前狀態
        HandleStateChanged(flow.CurrentStateType);
    }

    void OnDestroy()
    {
        if (flow != null) flow.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameStateType type)
    {
        if (root != null) root.SetActive(type == GameStateType.Playing);
    }
}
