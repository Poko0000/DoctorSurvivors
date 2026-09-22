using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 背包網格的資料與邏輯核心。完全不依賴 UI,方便單獨測試。
/// 用二維陣列記錄每一格被哪個 ItemInstance 佔用(null 表示空格)。
/// </summary>
public class BackpackGridModel
{
    public readonly int Width;
    public readonly int Height;

    private readonly ItemInstance[,] _cells;
    private readonly List<ItemInstance> _items = new List<ItemInstance>();

    public BackpackGridModel(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new ItemInstance[width, height];
    }

    public IReadOnlyList<ItemInstance> Items => _items;

    /// <summary>
    /// 檢查某個形狀放在指定錨點座標時是否合法(沒超出邊界、沒跟其他物品重疊)。
    /// ignoreItem 用在「拖曳中的物品自己不算碰撞」的情況。
    /// </summary>
    public bool CanPlace(ItemShape shape, Vector2Int origin, ItemInstance ignoreItem = null)
    {
        foreach (var cell in shape.Cells)
        {
            Vector2Int world = origin + cell;

            if (world.x < 0 || world.x >= Width || world.y < 0 || world.y >= Height)
                return false;

            var occupant = _cells[world.x, world.y];
            if (occupant != null && occupant != ignoreItem)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 嘗試放置物品,成功則回傳 true 並更新格子佔用狀態
    /// </summary>
    public bool TryPlace(ItemInstance item, Vector2Int origin)
    {
        var shape = item.GetCurrentShape();
        if (!CanPlace(shape, origin, ignoreItem: item))
            return false;

        // 如果物品已經在網格中,先清掉舊的佔用格
        ClearOccupancy(item);

        foreach (var cell in shape.Cells)
        {
            Vector2Int world = origin + cell;
            _cells[world.x, world.y] = item;
        }

        item.OriginCell = origin;
        if (!_items.Contains(item))
            _items.Add(item);

        return true;
    }

    /// <summary>
    /// 從網格上移除物品(例如撿起來準備拖曳,或丟棄)
    /// </summary>
    public void Remove(ItemInstance item)
    {
        ClearOccupancy(item);
        _items.Remove(item);
    }

    private void ClearOccupancy(ItemInstance item)
    {
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
            if (_cells[x, y] == item)
                _cells[x, y] = null;
    }

    public ItemInstance GetItemAt(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= Width || cell.y < 0 || cell.y >= Height)
            return null;
        return _cells[cell.x, cell.y];
    }

    /// <summary>
    /// 嘗試在網格中找到第一個能放下該形狀的位置(常用於「自動整理」或撿到新物品時自動找空位)
    /// </summary>
    public bool TryFindFreeSpot(ItemShape shape, out Vector2Int result)
    {
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            var origin = new Vector2Int(x, y);
            if (CanPlace(shape, origin))
            {
                result = origin;
                return true;
            }
        }
        result = default;
        return false;
    }
}
