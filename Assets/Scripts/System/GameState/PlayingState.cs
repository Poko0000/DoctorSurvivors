using UnityEngine;

// 正常遊玩中:累計存活時間、監聽玩家死亡
public class PlayingState : IGameState
{
    private readonly GameFlowController flow;
    private PlayerHealthHandler playerHealth;

    public GameStateType Type => GameStateType.Playing;

    public PlayingState(GameFlowController flow)
    {
        this.flow = flow;
    }

    public void Enter()
    {
        // 從 PlayerController.Instance 取得 PlayerHealthHandler 存到 playerHealth
        // 找不到時 LogError 並 return
        if(PlayerController.Instance == null)
        {
            Debug.LogError("[PlayingState]找不到Player");
            return;
        }
        playerHealth = PlayerController.Instance.GetComponent<PlayerHealthHandler>();
        // HandlePlayerDead 訂閱 playerHealth.OnDead
        if (playerHealth == null)
        {
            Debug.LogError("[PlayingState] 玩家身上沒有 PlayerHealthHandler,不會觸發 GameOver");
            return;
        }
        playerHealth.OnDead += HandlePlayerDead;
    }

    public void Exit()
    {
        // playerHealth 不是 null 就取消訂閱 OnDead
        if(playerHealth != null)
        {
            playerHealth.OnDead -= HandlePlayerDead;
        }
    }

    public void Tick()
    {
        // 呼叫 flow.AddElapsedTime(Time.deltaTime)
        //   (升級暫停時 deltaTime 是 0,所以暫停期間不會計時)
        flow.AddElapsedTime(Time.deltaTime);
        // 如果 flow.ElapsedTime >= flow.SurviveDuration → flow.GoToVictory()
        if(flow.ElapsedTime >= flow.SurviveDuration)
        {
            flow.GoToVictory();
        }
    }

    private void HandlePlayerDead()
    {
        flow.GoToGameOver();
    }
}
