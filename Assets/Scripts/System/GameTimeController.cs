using System.Collections.Generic;
using UnityEngine;

// 全遊戲唯一可以修改 Time.timeScale 的地方
// 其他系統一律透過 PauseEventChannel 發送請求,不准自己改 timeScale
public class GameTimeController : MonoBehaviour
{
    // 單一
    public static GameTimeController Instance { get; private set; }
    [SerializeField] private PauseEventChannel pauseChannel;

    // 目前要求暫停的人。用 HashSet:同一個人重複要求也只算一次
    private readonly HashSet<object> pauseRequesters = new HashSet<object>();

    public bool IsPaused => pauseRequesters.Count > 0;

    void Awake()
    {
        // 確保此為系列中唯一
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // 訂閱 pauseChannel 的三個事件
        // 檢查 pauseChannel 是否為 null,null 時 LogError
        if(pauseChannel == null)
        {
            Debug.LogError("pauseChannel is null.");
            return;
        }
        pauseChannel.OnPauseRequested += HandlePauseRequest;
        pauseChannel.OnPauseReleased += HandlePauseRelease;
        pauseChannel.OnClearAll += HandleClearAll;

    }

    void OnDisable()
    {
        // 取消訂閱(OnEnable 已經報過錯,這裡直接 return)
        if(pauseChannel == null) return;

        pauseChannel.OnPauseRequested -= HandlePauseRequest;
        pauseChannel.OnPauseReleased -= HandlePauseRelease;
        pauseChannel.OnClearAll -= HandleClearAll;

    }

    private void HandlePauseRequest(object requester)
    {
        // 把 requester 加進 pauseRequesters
        pauseRequesters.Add(requester);
        Apply();
    }

    private void HandlePauseRelease(object requester)
    {
        // 把 requester 從 pauseRequesters 移除
        pauseRequesters.Remove(requester);
        Apply();
    }

    private void HandleClearAll()
    {
        // 清空 pauseRequesters
        pauseRequesters.Clear();
        Apply();
    }

    private void Apply()
    {
        // IsPaused 為 true → Time.timeScale = 0f,否則 = 1f
        if(IsPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
