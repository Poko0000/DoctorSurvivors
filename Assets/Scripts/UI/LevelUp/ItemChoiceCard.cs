using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoiceCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private Button button;

    private ItemData data;                 // 存這張卡片代表的物品
    private Action<ItemData> onClick;

    public void Setup(ItemData itemData, Action<ItemData> clickCallback)
    {
        data = itemData;
        onClick = clickCallback;
        // 把 data 的 icon、itemName、description 填到三個欄位
        icon.sprite = data.icon;
        nameText.text = data.itemName;
        descText.text = data.description;

        // 先 button.onClick.RemoveAllListeners()
        button.onClick.RemoveAllListeners();

        // button.onClick.AddListener(...) 在被點擊時呼叫 onClick(data)
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        onClick?.Invoke(data);
    }
}
