using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 結算畫面:監聽遊戲狀態,GameOver / Victory 時顯示結果與按鈕
// 只負責「顯示」與「把按鈕轉交給 GameFlowController」,不自己判斷勝負、不改 timeScale
public class ResultUI : MonoBehaviour
{
    [Header("面板")]
    [SerializeField] private GameObject panel;

    [Header("文字")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text killText;

    [Header("按鈕")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("標題內容")]
    [SerializeField] private string gameOverTitle = "GAME OVER";
    [SerializeField] private string victoryTitle = "VICTORY";

    private GameFlowController flow;
    private PlayerLevelHandler levelHandler;

    void Start()
    {
        // panel 先關掉
        if (panel != null) panel.SetActive(false);

        if(GameFlowController.Instance == null)
        {
            Debug.LogError("[ResultUI] 找不到GameFlowController");
            return;
        }
        flow = GameFlowController.Instance;
        // 訂閱 flow.OnStateChanged → HandleStateChanged
        flow.OnStateChanged += HandleStateChanged;

        // 兩個按鈕 AddListener
        if (restartButton != null) restartButton.onClick.AddListener(flow.Restart);
        if (menuButton != null) menuButton.onClick.AddListener(flow.BackToMenu);

        // 從 PlayerController.Instance 取得 PlayerLevelHandler 存到 levelHandler
        //   找不到只需要 LogWarning,不要 return(沒有等級資訊,結算畫面還是能用)
        if(PlayerController.Instance == null)
        {
            Debug.LogWarning("[ResultUI] 找不到PlayerController");     
        }
        else
        {
            levelHandler = PlayerController.Instance.GetComponent<PlayerLevelHandler>();
        }
        
    }

    void OnDestroy()
    {
        // flow 不是 null 就退訂 OnStateChanged
        if(flow != null)
        {
            flow.OnStateChanged -= HandleStateChanged;
            //兩個按鈕 RemoveListener
            if (restartButton != null) restartButton.onClick.RemoveListener(flow.Restart);
            if (menuButton != null) menuButton.onClick.RemoveListener(flow.BackToMenu);
        }       
    }

    private void HandleStateChanged(GameStateType type)
    {
        // 判斷 type
        switch(type)
        {
            case GameStateType.GameOver: Show(gameOverTitle);
            break;
            case GameStateType.Victory: Show(victoryTitle);
            break;
            default: Hide();
            break;
        }
    }

    private void Show(string title)
    {
        // titleText 設成 title
        titleText.text = title;
        // timeText 設成 "存活時間 " + FormatTime(flow.ElapsedTime)
        timeText.text = "Play Time: " + FormatTime(flow.ElapsedTime);
        // killText 顯示擊殺數
        if (killText != null) killText.text = "Kill: " + flow.KillCount;
        // levelText 顯示 "等級 Lv." + levelHandler.Level
        // null → levelText 顯示 "等級 --"
        if(levelHandler != null)
        {
            levelText.text = "Level Lv." + levelHandler.Level;
        }
        else
        {
            levelText.text = "Level --";
        }
        // panel 打開
        panel.SetActive(true);
    }

    private void Hide()
    {
        // panel 關掉
        panel.SetActive(false);
    }

    // 把秒數轉成 "mm:ss",例如 125.7 → "02:05"
    private string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);   // 用 Mathf.FloorToInt 取整數秒
        int minutes = totalSeconds / 60;                // 分 = 總秒數 / 60
        int secs    = totalSeconds % 60;                // 秒 = 總秒數 % 60
        return $"{minutes:00}:{secs:00}";               // 回傳 $"{分:00}:{秒:00}"   ← :00 代表補零到兩位數
    }
}
