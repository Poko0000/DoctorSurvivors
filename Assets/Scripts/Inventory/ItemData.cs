using UnityEngine;

public enum ShapeType
{
    Single,
    Line2Horizontal,
    Line3Vertical,
    Square2x2,
    LShape
}

/// <summary>
/// 物品的靜態資料定義。用 CreateAssetMenu 讓你在 Project 視窗右鍵就能建立物品資源,
/// 不用寫程式就能設計新道具。所有物品(不管是不是武器)都用同一個類型,
/// 用下面的 isWeapon 勾選框決定要不要啟用武器欄位——這樣你以後新增物品時,
/// 不需要知道「這個物品該用哪一種資源類型」,永遠都是同一個選單、同一個介面。
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Backpack/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本資料")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ShapeType shapeType = ShapeType.Single;

    [Header("武器效果(不是武器的話,以下欄位不用管)")]
    [Tooltip("勾選後,這個物品放進背包時會自動生成對應的武器並開始攻擊")]
    public bool isWeapon;
    [Tooltip("連結到你既有的武器系統,提供傷害/冷卻/彈幕Prefab等數值")]
    public WeaponData weaponData;

    // 以後要加其他種類的效果(例如「被動加成物品」),一樣的做法:
    // 加一個 [Header("被動效果")] + public bool isPassiveBoost + 對應的數值欄位,
    // 再到下面 OnEquipped/OnUnequipped 裡加一段 if(isPassiveBoost){...} 判斷即可,
    // 完全不用新增資源類型。

    public ItemShape GetBaseShape()
    {
        return shapeType switch
        {
            ShapeType.Single => ItemShape.Single(),
            ShapeType.Line2Horizontal => ItemShape.Line2Horizontal(),
            ShapeType.Line3Vertical => ItemShape.Line3Vertical(),
            ShapeType.Square2x2 => ItemShape.Square2x2(),
            ShapeType.LShape => ItemShape.LShape(),
            _ => ItemShape.Single()
        };
    }

    /// <summary>
    /// 物品被放進背包(裝備)時呼叫。目前只處理「是不是武器」,
    /// 之後要加其他效果類型,就在這裡多加一段 if 判斷。
    /// </summary>
    public void OnEquipped(ItemInstance instance, Transform ownerRoot)
    {
        if (!isWeapon) return;

        if (weaponData == null || weaponData.weaponPrefab == null)
        {
            Debug.LogWarning($"物品 [{itemName}] 勾選了 isWeapon,但沒有設定 weaponData 或 weaponPrefab。");
            return;
        }

        var weaponGO = Object.Instantiate(weaponData.weaponPrefab, ownerRoot);
        var weapon = weaponGO.GetComponent<IWeapon>();
        if (weapon != null)
        {
            weapon.data = weaponData; // 把數值餵給武器腳本,IWeapon.Update() 裡的冷卻/攻擊邏輯就會用這份資料
        }
        else
        {
            Debug.LogWarning($"武器 Prefab {weaponData.weaponPrefab.name} 上沒有找到 IWeapon 元件,武器不會運作。");
        }

        instance.EquippedGameObject = weaponGO;
    }

    /// <summary>
    /// 物品從背包被徹底移除(丟棄、賣掉、拆解)時呼叫,清掉 OnEquipped 產生的物件。
    /// </summary>
    public void OnUnequipped(ItemInstance instance, Transform ownerRoot)
    {
        if (instance.EquippedGameObject != null)
        {
            Object.Destroy(instance.EquippedGameObject);
            instance.EquippedGameObject = null;
        }
    }
}

/// <summary>
/// 場景/背包中實際存在的一個物品實例:記錄目前旋轉狀態、位置等動態資料。
/// ItemData 是「藍圖」,ItemInstance 是「這一個具體的道具」。
/// </summary>
[System.Serializable]
public class ItemInstance
{
    public ItemData Data;
    public int RotationSteps; // 0~3,表示順時針轉了幾次 90 度
    public Vector2Int OriginCell; // 放在背包裡的錨點座標

    // 執行期用:如果這個物品裝備後生成了對應的 GameObject(例如武器行為腳本),記錄在這裡,
    // 之後卸下裝備時才知道該刪除哪一個。純執行期狀態,不需要存檔,也不會被序列化。
    [System.NonSerialized] public GameObject EquippedGameObject;

    public ItemInstance(ItemData data)
    {
        Data = data;
        RotationSteps = 0;
    }

    public ItemShape GetCurrentShape()
    {
        var shape = Data.GetBaseShape();
        for (int i = 0; i < RotationSteps; i++)
            shape = shape.RotatedClockwise();
        return shape;
    }

    public void Rotate()
    {
        RotationSteps = (RotationSteps + 1) % 4;
    }
}