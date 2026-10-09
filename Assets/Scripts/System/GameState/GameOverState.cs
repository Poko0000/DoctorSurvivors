// 玩家死亡:暫停遊戲,等待玩家選「重來」或「回主選單」
public class GameOverState : IGameState
{
    private readonly GameFlowController flow;

    public GameStateType Type => GameStateType.GameOver;

    public GameOverState(GameFlowController flow)
    {
        this.flow = flow;
    }

    public void Enter()
    {
        // 請求暫停
        flow.PauseChannel?.RaisePauseRequest(this);
    }

    public void Exit()
    {
        // 解除暫停
        flow.PauseChannel?.RaisePauseRelease(this);
    }

    public void Tick()
    {
        // 結算畫面期間沒有每幀邏輯,留空即可
    }
}
