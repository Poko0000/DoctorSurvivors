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

    [Header("武器/被動效果生成在這個 Transform 底下(通常拖玩家角色本身)")]
    public Transform effectRoot;

    private void Start()
    {
        foreach (var data in startingItems)
        {
            SpawnItem(data);
        }
    }

    /// <summary>
    /// 生成一個新物品、放進背包,並觸發它的裝備效果(OnEquipped)。
    /// 這是唯一會呼叫 OnEquipped 的地方——物品在背包裡被拖曳、重新擺放位置時
    /// (DraggableItemUI 內部的 Model.Remove + TryPlace)不會、也不應該再觸發一次,
    /// 不然同一把武器會被重複生成。OnEquipped 只代表「這個物品第一次進入背包」。
    /// </summary>
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
        data.OnEquipped(instance, effectRoot);

        var go = Instantiate(itemUIPrefab, gridUI.CellsParent);
        var image = go.GetComponentInChildren<UnityEngine.UI.Image>();
        if (image != null) image.sprite = data.icon;

        var draggable = go.GetComponent<DraggableItemUI>();
        draggable.Initialize(instance, gridUI, rootCanvas);
        return draggable;
    }

    /// <summary>
    /// 把物品徹底從背包移除(丟棄/賣掉/拆解):觸發 OnUnequipped 清掉裝備效果
    /// (例如銷毀對應的武器物件),並刪除物品的 UI 物件本身。
    /// 之後如果要做「拖出背包外= 丟棄」的功能,就是在那個時機點呼叫這個方法。
    /// </summary>
    public void DiscardItem(DraggableItemUI itemUI)
    {
        var instance = itemUI.Item;
        gridUI.Model.Remove(instance);
        instance.Data.OnUnequipped(instance, effectRoot);
        Destroy(itemUI.gameObject);
    }
}