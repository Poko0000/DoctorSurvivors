using UnityEngine;

/// <summary>
/// 通用元件:讓這個物件的 RectTransform 自動跟著另一個 RectTransform(target)的大小走,
/// 並可以在四邊分別加上留白(padding),做出外框效果。
/// 這個腳本完全不知道「背包」「格子」這些概念,只單純處理「跟著目標縮放」,
/// 所以除了背包背景,以後任何「外框要跟著內容大小變化」的 UI 都能直接掛這個腳本重複使用。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class AutoFitBackground : MonoBehaviour
{
    [Tooltip("要跟隨大小的目標,通常是這個物件的父層 Panel")]
    public RectTransform target;

    [Header("四邊留白(可分開設定,做出不對稱外框)")]
    public float paddingLeft = 0f;
    public float paddingRight = 0f;
    public float paddingTop = 0f;
    public float paddingBottom = 0f;

    private RectTransform _self;

    private void Awake()
    {
        _self = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 在編輯器裡改 padding 數值時,不用按 Play 就能即時看到效果。
    /// 只調整 padding、不改 target 大小的情況下,單靠這個就夠了。
    /// </summary>
    private void OnValidate()
    {
        if (_self == null) _self = GetComponent<RectTransform>();
        Refresh();
    }

    /// <summary>
    /// 重新計算並套用大小/位置。當 target 的大小是「由程式碼動態改變」時
    /// (例如背包格數變了),OnValidate 不會自動觸發,要由外部(例如 BackpackGridUI)
    /// 在改完 target 大小之後主動呼叫這個方法,通知背景該跟著更新了。
    /// 前提跟之前一樣:target 跟這個物件本身的錨點/軸心都必須是左上角。
    /// </summary>
    public void Refresh()
    {
        if (_self == null || target == null) return;

        _self.anchorMin = new Vector2(0, 1);
        _self.anchorMax = new Vector2(0, 1);
        _self.pivot = new Vector2(0, 1);

        _self.sizeDelta = new Vector2(
            target.rect.width + paddingLeft + paddingRight,
            target.rect.height + paddingTop + paddingBottom);

        // 位置往左上偏移(paddingLeft, paddingTop),讓 target 的區域準確落在背景「扣掉留白」的位置
        _self.anchoredPosition = new Vector2(-paddingLeft, paddingTop);
    }
}
