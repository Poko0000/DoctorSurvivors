using System;

// 純 C# 類別(不是 MonoBehaviour),只負責「持有目前狀態 + 切換」
public class GameStateMachine
{
    public IGameState CurrentState { get; private set; }

    // 切換完成後通知外部(新狀態的類型)
    public event Action<GameStateType> OnStateChanged;

    public void ChangeState(IGameState next)
    {
        // next 是 null,或跟 CurrentState 是同一個 → 直接 return
        if(next == null || next == CurrentState) return;
        
        // 如果 CurrentState 不是 null,先呼叫它的 Exit()
        CurrentState?.Exit();
        //CurrentState = next,然後呼叫 next.Enter()
        CurrentState = next;
        CurrentState.Enter();
        // 觸發 OnStateChanged,傳入 next.Type
        OnStateChanged?.Invoke(next.Type);
    }

    public void Tick()
    {
        // CurrentState 不是 null 就呼叫它的 Tick()
        CurrentState?.Tick();
    }
}
