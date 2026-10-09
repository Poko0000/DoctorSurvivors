using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpRewardSystem : MonoBehaviour
{
    public static LevelUpRewardSystem Instance { get; private set; }
    [SerializeField] private ItemPool itemPool;
    [SerializeField] private int choiceCount = 3;

    [Header("Event")]
    [SerializeField] PauseEventChannel pauseEvent;

    private PlayerLevelHandler levelHandler;
    private int pendingChoices;     // 還沒處理的升級次數
    private bool isChoosing;        // 目前是否正在等玩家選

    // 給 UI 訂閱:有新的三選一要顯示了
    public event Action<List<ItemData>> OnChoicesReady;
    // 給 UI 訂閱:所有選擇都做完了(之後用來關閉選單、恢復遊戲)
    public event Action OnAllChoicesDone;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogError("[Reward] 找不到 PlayerController.Instance,升級獎勵不會運作。");
            return;
        }

        levelHandler = PlayerController.Instance.GetComponent<PlayerLevelHandler>();
        
        if (levelHandler == null)
        {
            Debug.LogError("[Reward] 玩家身上沒有 PlayerLevelHandler,升級獎勵不會運作。");
            return;
        }

        levelHandler.OnlevelUp += HandleLevelUp;

        if (pauseEvent == null)
        {
            Debug.LogError("[Reward] 沒有指定 PauseEventChannel,升級時遊戲不會暫停。");
        }
    }

    void OnDestroy()
    {
        if (levelHandler == null) return;
        levelHandler.OnlevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        // pendingChoices 加 1
        pendingChoices++;
        // 如果現在沒有在選(isChoosing 為 false),就呼叫 ShowNextChoice()
        if(!isChoosing)
        {
            ShowNextChoice();
        }
        
    }

    private void ShowNextChoice()
    {
        // 從 itemPool 抽 choiceCount 個
        List<ItemData> choices = itemPool.GetRandomChoices(choiceCount);

        // 如果抽不到任何東西,不要卡住:交給 FinishChoosing 收尾
        if (choices.Count == 0)
        {
            FinishChoosing();
            return;
        }

        // isChoosing = true,觸發 OnChoicesReady 並把抽到的清單傳出去
        isChoosing = true;
        pauseEvent.RaisePauseRequest(this);
        OnChoicesReady?.Invoke(choices);
    }

    // 給 UI 在玩家點擊後呼叫
    public void SelectItem(ItemData chosen)
    {
        if (!isChoosing) return;
        // 把 chosen 交給背包:有空位就放進去,滿了就掉在玩家腳邊
        BackpackManager.Instance.AddOrDrop(chosen);
        //  pendingChoices 減 1
        pendingChoices--;
        // 如果還有 pendingChoices,就 ShowNextChoice();
        //          否則交給 FinishChoosing 收尾
        if(pendingChoices > 0) 
        {
            ShowNextChoice();
        }
        else
        {
            FinishChoosing();
        }

    }

    // 結束整輪選擇:關閉選單、恢復遊戲
    // 正常選完、或中途抽不到道具,都走這裡收尾
    private void FinishChoosing()
    {
        pendingChoices = 0;
        isChoosing = false;
        OnAllChoicesDone?.Invoke();
        pauseEvent.RaisePauseRelease(this);
    }
}
