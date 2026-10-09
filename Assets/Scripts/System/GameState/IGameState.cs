// State Pattern:每個狀態把自己「進入、離開、每幀要做的事」包在自己的類別裡
// 狀態機只負責切換,不需要知道每個狀態的細節
public interface IGameState
{
    GameStateType Type { get; }

    void Enter();   // 切換進這個狀態時呼叫一次
    void Exit();    // 離開這個狀態時呼叫一次
    void Tick();    // 停留在這個狀態時,每幀呼叫
}
