// 存活到時間結束:暫停遊戲,等待玩家選「重來」或「回主選單」
public class VictoryState : IGameState
{
    private readonly GameFlowController flow;

    public GameStateType Type => GameStateType.Victory;

    public VictoryState(GameFlowController flow)
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
    }

    // 思考題:GameOverState 和 VictoryState 的 Enter/Exit 幾乎一樣
    //   如果之後要加共同行為(例如停止生怪、播結算音效),
    //   可以抽一個 abstract class ResultState : IGameState 讓兩者繼承(Template Method Pattern)
    //   先把兩個寫完、能動之後再考慮重構
}
