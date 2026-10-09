using System;
using UnityEngine;

// 戰鬥場景的流程總管:建立狀態機與各狀態,提供切換入口給狀態和 UI 使用
// 只放在戰鬥場景,不需要 DontDestroyOnLoad(每次進戰鬥都重新建立)
public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance { get; private set; }

    [Header("規則")]
    [SerializeField] private float surviveDuration = 600f;   // 存活幾秒算勝利

    [Header("事件")]
    [SerializeField] private PauseEventChannel pauseChannel;

    [Header("場景")]
    [SerializeField] private SceneData battleScene;   // 重來時載入
    [SerializeField] private SceneData menuScene;     // 回主選單時載入

    // 給狀態讀取
    public PauseEventChannel PauseChannel => pauseChannel;
    public float SurviveDuration => surviveDuration;
    public float ElapsedTime { get; private set; }

    // 給 UI 訂閱(HUD、結算畫面)
    public event Action<GameStateType> OnStateChanged;
    public GameStateType CurrentStateType => stateMachine?.CurrentState != null ? stateMachine.CurrentState.Type : GameStateType.None;

    private GameStateMachine stateMachine;
    private PlayingState playingState;
    private GameOverState gameOverState;
    private VictoryState victoryState;

    void Awake()
    {
        //Singleton
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 建立 stateMachine 和三個狀態
        stateMachine  = new GameStateMachine();
        playingState = new PlayingState(this);
        gameOverState = new GameOverState(this);
        victoryState = new VictoryState(this);

        // 把 stateMachine.OnStateChanged 轉發給自己的 OnStateChanged
        stateMachine.OnStateChanged += type => OnStateChanged?.Invoke(type);
    }

    void Start()
    {
        // 檢查 pauseChannel,null 就 LogError
        if(pauseChannel == null)
        {
            Debug.LogError("[GameFlow] 沒有指定 PauseEventChannel,結算時遊戲不會暫停。");
        }

        // ElapsedTime 歸零,切換到 playingState
        ElapsedTime = 0f;
        stateMachine.ChangeState(playingState);

    }

    void Update()
    {
        // state tick
        stateMachine.Tick();
    }

    void OnDestroy()
    {
        // 只有「自己就是 Instance」時才清掉
        if (Instance == this)
        {
            Instance = null;
        }
        
    }

    // ===== 給狀態呼叫 =====

    public void AddElapsedTime(float deltaTime)
    {
        // ElapsedTime 加上 deltaTime
        ElapsedTime += deltaTime;
    }

    public void GoToGameOver()
    {
        // 切換到 gameOverState
        stateMachine.ChangeState(gameOverState);
    }

    public void GoToVictory()
    {
        //切換到 victoryState
        stateMachine.ChangeState(victoryState);
    }

    // ===== 給結算 UI 的按鈕呼叫 =====

    public void Restart()
    {
        // 重新載入戰鬥場景,再玩一局
        if (SceneController.Instance == null)
        {
            Debug.LogError("[GameFlow] 找不到 SceneController,請從主選單進入遊戲。");
            return;
        }
        SceneController.Instance.LoadScene(battleScene);
    }

    public void BackToMenu()
    {
        // 載入主選單場景
        if (SceneController.Instance == null)
        {
            Debug.LogError("[GameFlow] 找不到 SceneController,請從主選單進入遊戲。");
            return;
        }
        SceneController.Instance.LoadScene(menuScene);
    }
}
