using UnityEngine;

/// <summary>
/// 示範用的整合腳本:負責在遊戲開始時,把物品資料實例化成畫面上可拖曳的物品UI,
/// 並放進背包網格裡。實務上你會從「戰利品系統」「商店」呼叫類似的 SpawnItem 方法。
/// </summary>
public class BackpackManager : MonoBehaviour
{
    public BackpackGridUI gridUI;
    public Canvas rootCanvas;
    public GameObject itemUIPrefab; // 需包含 Image(顯示icon) + DraggableItemUI 元件
    public ItemData[] startingItems;

    private void Start()
    {
        foreach (var data in startingItems)
        {
            SpawnItem(data);
        }
    }

    public DraggableItemUI SpawnItem(ItemData data)
    {
        var instance = new ItemInstance(data);
        var shape = instance.GetCurrentShape();

        if (!gridUI.Model.TryFindFreeSpot(shape, out var spot))
        {
            Debug.LogWarning($"背包空間不足,無法放入物品:{data.itemName}");
            return null;
        }

        gridUI.Model.TryPlace(instance, spot);

        var go = Instantiate(itemUIPrefab, gridUI.transform);
        var image = go.GetComponentInChildren<UnityEngine.UI.Image>();
        if (image != null) image.sprite = data.icon;

        var draggable = go.GetComponent<DraggableItemUI>();
        draggable.Initialize(instance, gridUI, rootCanvas);
        return draggable;
    }
}
