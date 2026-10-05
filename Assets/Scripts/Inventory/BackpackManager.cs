using UnityEngine;

/// <summary>
/// 示範用的整合腳本:負責在遊戲開始時,把物品資料實例化成畫面上可拖曳的物品UI,
/// 並放進背包網格裡。實務上你會從「戰利品系統」「商店」呼叫類似的 SpawnItem 方法。
/// </summary>
public class BackpackManager : MonoBehaviour
{
    // 跟專案裡 PlayerController 一樣的單例寫法,方便 DraggableItemUI、WorldItemPickup
    // 這些不方便直接持有參照的腳本,能用 BackpackManager.Instance 呼叫到它。
    public static BackpackManager Instance { get; private set; }

    public BackpackGridUI gridUI;
    public Canvas rootCanvas;
    public GameObject itemUIPrefab; // 需包含 Image(顯示icon) + DraggableItemUI 元件
    public ItemData[] startingItems;

    [Header("武器/被動效果生成在這個 Transform 底下(通常拖玩家角色本身)")]
    public Transform effectRoot;

    [Header("丟棄到地上的設定")]
    [Tooltip("掉在地上的物品外觀,需掛 WorldItemPickup.cs + Collider2D(Trigger)")]
    public GameObject worldItemPickupPrefab;
    [Tooltip("丟棄物生成的位置,通常拖玩家角色;不指定就用 BackpackManager 自己這個物件的位置")]
    public Transform dropOrigin;

    private void Awake()
    {
        Instance = this;
    }

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
    /// 把物品徹底從背包移除(賣掉/拆解等不需要留下世界物件的情況):觸發 OnUnequipped
    /// 清掉裝備效果(例如銷毀對應的武器物件),並刪除物品的 UI 物件本身。
    /// </summary>
    public void DiscardItem(DraggableItemUI itemUI)
    {
        var instance = itemUI.Item;
        gridUI.Model.Remove(instance);
        instance.Data.OnUnequipped(instance, effectRoot);
        Destroy(itemUI.gameObject);
    }

    /// <summary>
    /// 把背包物品丟到地上:先走 DiscardItem 的流程(卸除效果、移除UI),
    /// 再額外生成一個場景裡的掉落物(WorldItemPickup),記住這是「哪一種」物品,
    /// 這樣玩家之後走過去才能把它撿回來。
    /// 由 DraggableItemUI.OnEndDrag 在「拖出背包範圍外」時呼叫。
    /// </summary>
    public void DropItemToWorld(DraggableItemUI itemUI)
    {
        var data = itemUI.Item.Data; // 要在 DiscardItem 把 itemUI 銷毀之前,先記住它是什麼物品
        DiscardItem(itemUI);
        SpawnWorldPickup(data);
    }

    /// <summary>
    /// 在世界座標生成一個掉落物。目前簡化成「生成在玩家腳邊,加一點隨機偏移」,
    /// 不處理滑鼠拖到的位置對應到世界座標的複雜換算(那牽涉到你的攝影機設定方式)。
    /// </summary>
    private void SpawnWorldPickup(ItemData data)
    {
        if (worldItemPickupPrefab == null)
        {
            Debug.LogWarning("沒有指定 worldItemPickupPrefab,物品會直接消失,不會掉在地上。");
            return;
        }

        Vector3 origin = dropOrigin != null ? dropOrigin.position : transform.position;
        Vector3 spawnPos = origin + (Vector3)(Random.insideUnitCircle * 0.5f); // 隨機偏移,避免多個掉落物疊在同一點

        var worldGO = Instantiate(worldItemPickupPrefab, spawnPos, Quaternion.identity);
        var pickup = worldGO.GetComponent<WorldItemPickup>();
        if (pickup != null)
        {
            pickup.itemData = data;
        }
        else
        {
            Debug.LogWarning($"worldItemPickupPrefab 上沒有找到 WorldItemPickup 元件,玩家不會撿得起來。");
        }

        // 順便把地上的外觀換成這個物品的 icon,不然不管丟什麼下去,地上看起來都長一樣
        var renderer = worldGO.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null && data.icon != null)
        {
            renderer.sprite = data.icon;
        }
    }
}