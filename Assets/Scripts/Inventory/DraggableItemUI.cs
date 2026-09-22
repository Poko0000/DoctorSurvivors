using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 掛在每個「物品格子圖示」的 GameObject 上。實作 Unity EventSystem 的拖曳介面,
/// 處理:開始拖曳、拖曳中即時預覽、放開時判定能否放置、按右鍵旋轉。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemInstance Item { get; private set; }
    public BackpackGridUI GridUI { get; private set; }

    private RectTransform _rectTransform;
    private Canvas _rootCanvas;
    private Vector2 _pointerOffsetInCells; // 抓取時滑鼠在物品哪一格上,用來讓拖曳更自然
    private CanvasGroup _canvasGroup;

    public void Initialize(ItemInstance item, BackpackGridUI gridUI, Canvas rootCanvas)
    {
        Item = item;
        GridUI = gridUI;
        _rootCanvas = rootCanvas;
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        RefreshVisualSize();
        SnapToGridPosition();
    }

    /// <summary>
    /// 根據目前形狀更新這個 UI 元件的寬高(像素),旋轉後形狀改變時要呼叫
    /// </summary>
    public void RefreshVisualSize()
    {
        var shape = Item.GetCurrentShape();
        _rectTransform.sizeDelta = new Vector2(shape.Width * GridUI.cellSize, shape.Height * GridUI.cellSize);
        _rectTransform.rotation = Quaternion.identity; // 我們旋轉的是「形狀資料」,不是視覺角度
                                                        // 若想要圖示本身也跟著轉90度,可在這裡另外套用 icon 的 rotation
    }

    public void SnapToGridPosition()
    {
        _rectTransform.anchoredPosition = GridUI.CellToAnchoredPosition(Item.OriginCell);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false; // 拖曳時讓射線穿透自己,才能偵測到下方的格子
        GridUI.Model.Remove(Item); // 先從邏輯網格移除,拖曳過程不佔用原位置

        // 記錄滑鼠抓取點落在物品的第幾格,拖曳時用這個偏移量去對齊
        GridUI.TryScreenPointToCell(eventData.position, eventData.pressEventCamera, Vector2.zero, out var pointerCell);
        Vector2Int grabCellOffset = pointerCell - Item.OriginCell;
        _pointerOffsetInCells = new Vector2(grabCellOffset.x, grabCellOffset.y);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 讓物品圖示直接跟著滑鼠移動(用世界座標避免父層縮放造成的偏差)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
        _rectTransform.position = _rootCanvas.transform.TransformPoint(localPoint);

        // 計算目前指標對應到哪個格子,並顯示放置預覽
        if (GridUI.TryScreenPointToCell(eventData.position, eventData.pressEventCamera, _pointerOffsetInCells, out var cell))
        {
            var shape = Item.GetCurrentShape();
            bool valid = GridUI.Model.CanPlace(shape, cell, ignoreItem: Item);
            GridUI.ShowPlacementPreview(shape, cell, valid);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        GridUI.HideAllHighlights();

        bool placed = false;
        if (GridUI.TryScreenPointToCell(eventData.position, eventData.pressEventCamera, _pointerOffsetInCells, out var cell))
        {
            placed = GridUI.Model.TryPlace(Item, cell);
        }

        if (!placed)
        {
            // 放不下就嘗試找任意空位歸位,再不行就退回原位(視需求也可以讓物品「掉出背包」)
            var shape = Item.GetCurrentShape();
            if (GridUI.Model.TryFindFreeSpot(shape, out var fallback))
                GridUI.Model.TryPlace(Item, fallback);
        }

        SnapToGridPosition();
    }

    /// <summary>
    /// 右鍵點擊旋轉物品(Backpack Hero 裡常見的操作)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        var previousOrigin = Item.OriginCell;
        GridUI.Model.Remove(Item);
        Item.Rotate();

        var newShape = Item.GetCurrentShape();
        // 旋轉後優先嘗試原位放置,放不下就找別的空位,再不行就轉回去
        if (GridUI.Model.CanPlace(newShape, previousOrigin))
        {
            GridUI.Model.TryPlace(Item, previousOrigin);
        }
        else if (GridUI.Model.TryFindFreeSpot(newShape, out var spot))
        {
            GridUI.Model.TryPlace(Item, spot);
        }
        else
        {
            // 真的放不下,轉回原本的角度
            Item.RotationSteps = (Item.RotationSteps + 3) % 4;
            GridUI.Model.TryPlace(Item, previousOrigin);
        }

        RefreshVisualSize();
        SnapToGridPosition();
    }
}
