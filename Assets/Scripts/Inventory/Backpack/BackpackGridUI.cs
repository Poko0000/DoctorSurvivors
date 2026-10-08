using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包的視覺呈現層。放在一個 RectTransform 上(通常是 Canvas 底下的一個 Panel),
/// 負責:畫出格線背景、把「螢幕座標」換算成「格子座標」、顯示放置預覽(綠色/紅色)。
/// 格子位置全部手動計算(不使用 GridLayoutGroup),好處是完全掌控每一格的
/// 顯示/隱藏邏輯,不會有 LayoutGroup 因為子物件啟用狀態改變而重新排列的副作用。
/// </summary>
public class BackpackGridUI : MonoBehaviour
{
    [Header("網格設定")]
    public int width = 8;
    public int height = 6;
    public float cellSize = 64f; // 每一格本身的像素大小(不含間距)
    public Vector2 cellSpacing = Vector2.zero; // 格子之間的間距(x=水平間距, y=垂直間距),0 就是完全貼在一起

    [Header("固定顯示的背景格(格子底色/邊框)")]
    public GameObject cellBackgroundPrefab; // 一個簡單的 Image,例如帶邊框的淺色方塊,平常就會一直顯示

    [Header("整個背包面板的裝飾背景(可選,例如外框圖片)")]
    [Tooltip("拖入一個掛了 AutoFitBackground.cs 的 Image 物件,當作整片背包的底圖/外框。\n" +
             "它必須是 BackpackPanel 的子物件,而且要放在 Hierarchy 最上面(第一個子物件),\n" +
             "這樣疊圖順序才會在所有格子的最後面。留白(padding)請直接在 AutoFitBackground 上設定。")]
    public AutoFitBackground panelBackground;

    [Header("預覽用的格子高亮圖")]
    public GameObject cellHighlightPrefab; // 一個簡單的 Image,顏色可切換
    public Color validColor = new Color(0.3f, 1f, 0.3f, 0.5f);
    public Color invalidColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    [Header("整理用的容器(可選)")]
    [Tooltip("所有動態生成的東西(背景格、高亮格、物品)都會放進這個容器底下,\n" +
             "Hierarchy 才不會被塞爆一堆物件。留空的話會直接生成在 BackpackPanel 自己底下(舊行為)。")]
    public RectTransform content;

    public BackpackGridModel Model { get; private set; }
    public RectTransform RectTransform { get; private set; }

    /// <summary>
    /// 動態生成物件實際要放的父層——有指定 content 就用 content,沒有就退回用 BackpackPanel
    /// 自己(transform)。BackpackManager 生成物品時也要用這個,物品才會跟格子收在同一個地方。
    /// </summary>
    public Transform CellsParent => content != null ? (Transform)content : transform;

    // 「一格」在畫面上實際跨越的距離 = 格子本身大小 + 間距。
    // 所有座標換算(格子位置、螢幕座標轉格子、物品尺寸)都要用這個值,不能直接用 cellSize,
    // 不然只要 cellSpacing 不是 0,物品跟格子位置就會對不齊。
    private float StepX => cellSize + cellSpacing.x;
    private float StepY => cellSize + cellSpacing.y;

    private GameObject[,] _highlightCells;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Model = new BackpackGridModel(width, height);
        ResizePanelToFitGrid(); // Panel 自己的大小,直接算成剛好裝下 width x height 個格子
        BuildBackgroundGrid(); // 先生成背景,疊圖順序才會在 highlight 下面
        BuildHighlightGrid();
        HideAllHighlights();
    }

    /// <summary>
    /// 在編輯器裡修改 width / height / cellSize / cellSpacing 時(不用按 Play),
    /// Unity 會自動呼叫這個方法,讓你在 Scene 視窗就能即時看到 Panel 大小的變化,
    /// 不用等執行才知道調得對不對。
    /// </summary>
    private void OnValidate()
    {
        if (RectTransform == null)
            RectTransform = GetComponent<RectTransform>();

        ResizePanelToFitGrid();
    }

    /// <summary>
    /// 把 BackpackPanel 自己的 RectTransform 尺寸,設成剛好裝得下 width x height 個格子
    /// (含間距)的大小。前提是 Panel 本身的錨點要是左上角、不是拉伸模式(anchorMin != anchorMax
    /// 的拉伸模式下,sizeDelta 不會直接等於實際大小,設定會失準)。
    /// 改完大小後如果有指定 panelBackground,順便通知它「目標大小變了,重新跟一次」——
    /// 這個方法完全不用知道 AutoFitBackground 內部怎麼算 padding,只要呼叫 Refresh() 就好。
    /// </summary>
    private void ResizePanelToFitGrid()
    {
        if (RectTransform == null) return;

        RectTransform.sizeDelta = CellsToPixelSize(width, height);

        // content 容器要跟 BackpackPanel 完全疊在一起(同樣左上角、零偏移、一樣大小),
        // 這樣裡面的格子/物品用「相對 content 的座標」算出來的位置,數值上會跟
        // 「相對 BackpackPanel 的座標」完全一致,不用另外處理座標轉換。
        if (content != null)
        {
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(0, 1);
            content.pivot = new Vector2(0, 1);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = RectTransform.sizeDelta;
        }

        if (panelBackground != null)
        {
            panelBackground.target = RectTransform; // 自動指定跟隨目標就是這個 Panel 自己,不用你手動在 Inspector 拖
            panelBackground.Refresh();
        }
    }

    /// <summary>
    /// 生成每一格固定顯示的背景方塊(格線/底色),不會被拖曳邏輯操控,單純視覺用。
    /// </summary>
    private void BuildBackgroundGrid()
    {
        if (cellBackgroundPrefab == null) return; // 沒指定就跳過,不強制要求

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            var go = Instantiate(cellBackgroundPrefab, CellsParent);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x * StepX, -y * StepY);
            rt.sizeDelta = new Vector2(cellSize, cellSize); // 方塊本身還是 cellSize,間距靠位置間隔製造出來

            // 背景格不需要接收滑鼠事件,關掉 raycast 避免擋到物品的拖曳判定
            var img = go.GetComponent<Image>();
            if (img != null) img.raycastTarget = false;
        }
    }

    private void BuildHighlightGrid()
    {
        _highlightCells = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            var go = Instantiate(cellHighlightPrefab, CellsParent);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x * StepX, -y * StepY);
            rt.sizeDelta = new Vector2(cellSize, cellSize);
            _highlightCells[x, y] = go;
        }
    }

    /// <summary>
    /// 把「螢幕座標(例如滑鼠位置)」換算成「這個格子錨點的格子座標」。
    /// dragOffset 是滑鼠抓取物品時,相對物品左上角的偏移(讓拖曳手感自然)。
    /// 命中判定以 Step(格子+間距)為一個單位,間距本身也算在該格的範圍內,
    /// 這樣滑鼠停在間距的空隙上時,仍然會判定成離它最近的那一格,手感比較好抓。
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
        int cellX = Mathf.FloorToInt(topLeftLocal.x / StepX - pivotOffsetCells.x);
        int cellY = Mathf.FloorToInt(-topLeftLocal.y / StepY - pivotOffsetCells.y);

        cell = new Vector2Int(cellX, cellY);
        return true;
    }

    public Vector2 CellToAnchoredPosition(Vector2Int cell)
    {
        return new Vector2(cell.x * StepX, -cell.y * StepY);
    }

    /// <summary>
    /// 把「物品佔用幾格寬、幾格高」換算成它在畫面上實際該有的像素尺寸。
    /// 物品內部跨越的格子也包含間距(例如 2 格寬 = 2 個 cellSize 加上中間 1 條 spacing),
    /// 這樣物品的視覺大小才會跟底下的格子背景剛好對齊,不會因為有間距而露出縫隙或蓋過頭。
    /// </summary>
    public Vector2 CellsToPixelSize(int cellsWide, int cellsHigh)
    {
        float w = cellsWide * cellSize + Mathf.Max(0, cellsWide - 1) * cellSpacing.x;
        float h = cellsHigh * cellSize + Mathf.Max(0, cellsHigh - 1) * cellSpacing.y;
        return new Vector2(w, h);
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
        for (int y = 0; y < height; y++)
            _highlightCells[x, y].SetActive(false);
    }
}