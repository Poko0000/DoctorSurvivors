using System;
using UnityEngine;

// 事件頻道(Observer Pattern 的 ScriptableObject 版本)
// 發送方:任何需要暫停的系統,在 Inspector 拖入這個資產後呼叫 RaiseXxx()
// 接收方:GameTimeController 訂閱這裡的事件
// 好處:發送方完全不知道 GameTimeController 的存在
[CreateAssetMenu(fileName = "PauseEventChannel", menuName = "Scriptable Objects/Events/PauseEventChannel")]
public class PauseEventChannel : ScriptableObject
{
    //   - OnPauseRequested : Action<object>   (參數是「誰」要求暫停)
    public event Action<object> OnPauseRequested;
    //   - OnPauseReleased  : Action<object>   (參數是「誰」解除暫停)
    public event Action<object> OnPauseReleased;
    //   - OnClearAll       : Action           (切場景時清空所有請求)
    public event Action OnClearAll;


    // 給發送方呼叫的方法,裡面用 ?.Invoke 觸發對應事件
    public void RaisePauseRequest(object requester)
    {
        OnPauseRequested?.Invoke(requester);
    }

    public void RaisePauseRelease(object requester)
    {
        OnPauseReleased?.Invoke(requester);
    }

    public void RaiseClearAll()
    {
        OnClearAll?.Invoke();
    }
}
