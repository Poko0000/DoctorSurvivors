using TMPro;
using UnityEngine;

// 存活計時器:時間每幀都在變,沒有事件可以訂閱,所以用 Update 讀取
// 但只在「整數秒改變」時才改字,避免每幀產生新字串(GC)和 TMP 重新排版
public class SurvivalTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private bool countdown = false;   // 勾選:顯示剩餘時間;不勾:顯示已存活時間

    private GameFlowController flow;
    private int lastShownSecond = -1;

    void Start()
    {
        flow = GameFlowController.Instance;
        if (flow == null) Debug.LogError("[SurvivalTimerUI] 找不到 GameFlowController");
    }

    void Update()
    {
        if (flow == null || timeText == null) return;

        float seconds = countdown ? flow.SurviveDuration - flow.ElapsedTime : flow.ElapsedTime;
        int whole = Mathf.Max(0, Mathf.FloorToInt(seconds));
        if (whole == lastShownSecond) return;

        lastShownSecond = whole;
        timeText.text = TimeFormatter.ToMinutesSeconds(whole);
    }
}
