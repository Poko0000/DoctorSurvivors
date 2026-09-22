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
/// 物品的靜態資料定義。用 CreateAssetMenu 讓設計師在 Project 視窗右鍵建立物品資源,
/// 不用寫程式就能設計新道具(這是 Backpack Hero 這類遊戲常見的做法)。
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Backpack/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ShapeType shapeType = ShapeType.Single;

    // 之後要接戰鬥效果、被動效果等,可以在這裡擴充,例如：
    // public int damage;
    // public int block;
    // public List<ItemEffect> effects;

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
