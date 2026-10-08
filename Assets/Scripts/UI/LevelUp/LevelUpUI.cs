using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private ItemChoiceCard cardPrefab;

    private LevelUpRewardSystem rewardSystem;

    void Start()
    {
        panel.SetActive(false);

        rewardSystem = LevelUpRewardSystem.Instance;
        if (rewardSystem == null)
        {
            Debug.LogError("找不到 LevelUpRewardSystem,升級選單不會運作。");
            return;
        }

        rewardSystem.OnChoicesReady += ShowChoices;
        rewardSystem.OnAllChoicesDone += Hide;
    }

    void OnDestroy()
    {
        if (rewardSystem != null)
        {
            rewardSystem.OnChoicesReady -= ShowChoices;
            rewardSystem.OnAllChoicesDone -= Hide;
        }

        // 保險:如果在選單打開時切換場景,避免遊戲停在暫停狀態
        Time.timeScale = 1f;
    }

    private void ShowChoices(List<ItemData> choices)
    {
        Debug.Log("[UI] 收到選項");
        // 清掉上一輪的卡片(連續升級時會重複進來)
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in choices)
        {
            ItemChoiceCard card = Instantiate(cardPrefab, cardContainer);
            card.Setup(item, OnCardClicked);
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnCardClicked(ItemData chosen)
    {
        rewardSystem.SelectItem(chosen);
    }

    private void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
