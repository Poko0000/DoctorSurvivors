using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包的視覺呈現層。放在一個 RectTransform 上(通常是 Canvas 底下的一個 Panel),
/// 負責:畫出格線背景、把「螢幕座標」換算成「格子座標」、顯示放置預覽(綠色/紅色)。
/// </summary>
public class BackpackGridUI : MonoBehaviour
{
    [Header("網格設定")]
    public int width = 8;
    public int height = 6;
    public float cellSize = 64f; // 每一格的像素大小

    [Header("預覽用的格子高亮圖")]
    public GameObject cellHighlightPrefab; // 一個簡單的 Image,顏色可切換
    public Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    public Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    public BackpackGridModel Model { get; private set; }
    public RectTransform RectTransform { get; private set; }

    private GameObject[,] _highlightCells;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Model = new BackpackGridModel(width, height);
        BuildHighlightGrid();
        HideAllHighlights();
    }

    private void BuildHighlightGrid()
    {
        _highlightCells = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            var go = Instantiate(cellHighlightPrefab, transform);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
            rt.sizeDelta = new Vector2(cellSize, cellSize);
            _highlightCells[x, y] = go;
        }
    }

    /// <summary>
    /// 把「螢幕座標(例如滑鼠位置)」換算成「這個格子錨點的格子座標」。
    /// dragOffset 是滑鼠抓取物品時,相對物品左上角的偏移(讓拖曳手感自然)。
    /// </summary>
    public bool TryScreenPointToCell(Vector2 screenPoint, Camera uiCamera, Vector2 pivotOffsetCells, out Vector2Int cell)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, screenPoint, uiCamera, out var local))
        {
            cell = default;
            return false;
        }

        // RectTransform 的 local 座標原點在中心,這裡轉換成左上角為原點、往下往右為正的座標系
        Vector2 topLeftLocal = local - new Vector2(RectTransform.rect.xMin, RectTransform.rect.yMax);
        int cellX = Mathf.FloorToInt(topLeftLocal.x / cellSize - pivotOffsetCells.x);
        int cellY = Mathf.FloorToInt(-topLeftLocal.y / cellSize - pivotOffsetCells.y);

        cell = new Vector2Int(cellX, cellY);
        return true;
    }

    public Vector2 CellToAnchoredPosition(Vector2Int cell)
    {
        return new Vector2(cell.x * cellSize, -cell.y * cellSize);
    }

    /// <summary>
    /// 顯示某個形狀放在 origin 位置時的預覽高亮(綠色可放/紅色不可放)
    /// </summary>
    public void ShowPlacementPreview(ItemShape shape, Vector2Int origin, bool isValid)
    {
        HideAllHighlights();
        var color = isValid ? validColor : invalidColor;

        foreach (var cellOffset in shape.Cells)
        {
            var world = origin + cellOffset;
            if (world.x < 0 || world.x >= width || world.y < 0 || world.y >= height)
                continue;

            var go = _highlightCells[world.x, world.y];
            go.SetActive(true);
            var img = go.GetComponent<Image>();
            if (img != null) img.color = color;
        }
    }

    public void HideAllHighlights()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                    
                _highlightCells[x, y].SetActive(false);
            }
        }    
    }
}
