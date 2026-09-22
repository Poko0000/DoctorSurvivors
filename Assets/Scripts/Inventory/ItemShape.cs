using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 定義物品在背包格子中佔用的形狀(類似俄羅斯方塊)。
/// 用一組相對座標 (0,0) 為錨點來表示物品佔用哪些格子。
/// </summary>
[System.Serializable]
public struct ItemShape
{
    // 每個格子相對於錨點(左上角)的偏移量
    public List<Vector2Int> Cells;

    public ItemShape(IEnumerable<Vector2Int> cells)
    {
        Cells = cells.ToList();
    }

    /// <summary>
    /// 常見形狀範例,可依需求擴充
    /// </summary>
    public static ItemShape Single() => new ItemShape(new[] { new Vector2Int(0, 0) });

    public static ItemShape Line2Horizontal() => new ItemShape(new[]
    {
        new Vector2Int(0, 0), new Vector2Int(1, 0)
    });

    public static ItemShape Line3Vertical() => new ItemShape(new[]
    {
        new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2)
    });

    public static ItemShape Square2x2() => new ItemShape(new[]
    {
        new Vector2Int(0, 0), new Vector2Int(1, 0),
        new Vector2Int(0, 1), new Vector2Int(1, 1)
    });

    public static ItemShape LShape() => new ItemShape(new[]
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, 2),
        new Vector2Int(1, 2)
    });

    /// <summary>
    /// 順時針旋轉 90 度後回傳新的形狀(不修改原本資料)。
    /// 旋轉公式: (x, y) -> (-y, x),再做正規化讓座標不為負。
    /// </summary>
    public ItemShape RotatedClockwise()
    {
        var rotated = Cells.Select(c => new Vector2Int(-c.y, c.x)).ToList();
        return Normalize(rotated);
    }

    /// <summary>
    /// 將座標平移到左上角從 (0,0) 開始,避免旋轉後出現負座標
    /// </summary>
    private static ItemShape Normalize(List<Vector2Int> cells)
    {
        int minX = cells.Min(c => c.x);
        int minY = cells.Min(c => c.y);
        var normalized = cells.Select(c => new Vector2Int(c.x - minX, c.y - minY)).ToList();
        return new ItemShape(normalized);
    }

    public int Width => Cells.Count == 0 ? 0 : Cells.Max(c => c.x) + 1;
    public int Height => Cells.Count == 0 ? 0 : Cells.Max(c => c.y) + 1;
}
