using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpRewardSystem : MonoBehaviour
{
    public static LevelUpRewardSystem Instance;
    [SerializeField] private ItemPool itemPool;
    [SerializeField] private int choiceCount = 3;

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
        Debug.Log("[Reward] 已訂閱升級事件");
    }

void OnDestroy()
    {
        if (levelHandler == null) return;
        levelHandler.OnlevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int newLevel)
    {
        Debug.Log("[Reward] 收到升級");
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
        Debug.Log("[Reward] 抽到 " + choices.Count + " 個");

        // 如果抽不到任何東西,不要卡住:
        //         把 pendingChoices 歸零、isChoosing 設 false 然後直接 return
        if (choices.Count == 0)
        {
            // 沒東西可選:不要卡住
            pendingChoices = 0;
            isChoosing = false;
            return;
        }

        // isChoosing = true,觸發 OnChoicesReady 並把抽到的清單傳出去
        isChoosing = true;
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
        //          否則 isChoosing = false 並觸發 OnAllChoicesDone
        if(pendingChoices > 0) 
        {
            ShowNextChoice();
        }
        else
        {
            isChoosing = false;
            OnAllChoicesDone?.Invoke();
        }

    }
}
