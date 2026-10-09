using TMPro;
using UnityEngine;

// 擊殺數:訂閱 GameFlowController.OnKillCountChanged
public class KillCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text killText;
    [SerializeField] private string prefix = "擊殺 ";

    private GameFlowController flow;

    void Start()
    {
        flow = GameFlowController.Instance;
        if (flow == null)
        {
            Debug.LogError("[KillCountUI] 找不到 GameFlowController");
            return;
        }

        flow.OnKillCountChanged += Refresh;
        Refresh(flow.KillCount);
    }

    void OnDestroy()
    {
        if (flow != null) flow.OnKillCountChanged -= Refresh;
    }

    private void Refresh(int count)
    {
        if (killText != null) killText.text = prefix + count;
    }
}
