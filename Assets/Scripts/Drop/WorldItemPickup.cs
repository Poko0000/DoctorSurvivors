using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掛在「掉在地上的物品」Prefab 上(例如一個 SpriteRenderer + Collider2D)。
/// 玩家走進它的範圍(Trigger)時只會「進入可撿取狀態」,要按 F(Input Action「Pick」)才會撿回背包。
/// 如果背包已經滿了放不下,會留在地上,不會憑空消失,玩家晚點清出空間再回來撿。
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WorldItemPickup : MonoBehaviour
{
    [Tooltip("這個掉落物代表哪一個物品,由 BackpackManager.DropItemToWorld 在生成時自動設定,\n" +
             "如果是手動在場景裡擺放的戰利品(不是玩家丟棄的),也可以直接在 Inspector 指定。")]
    public ItemData itemData;

    [Tooltip("只有這個 Tag 的物件碰到才會觸發撿取,預設是 Player")]
    public string playerTag = "Player";

    [Tooltip("(選填)玩家在範圍內時顯示的提示,例如一個寫著「F 撿取」的子物件")]
    public GameObject pickupHint;

    // 目前玩家範圍內的所有掉落物,按 F 時從這裡挑最近的一個撿
    private static readonly List<WorldItemPickup> s_inRange = new List<WorldItemPickup>();

    private void Reset()
    {
        // 掉落物的碰撞體一定要是 Trigger,不然玩家會被它卡住、撞不過去
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Awake()
    {
        if (pickupHint != null) pickupHint.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (!s_inRange.Contains(this)) s_inRange.Add(this);
        if (pickupHint != null) pickupHint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        s_inRange.Remove(this);
        if (pickupHint != null) pickupHint.SetActive(false);
    }

    private void OnDisable()
    {
        s_inRange.Remove(this);
    }

    /// <summary>
    /// 由 PlayerController 在按下 F 時呼叫:撿起離 position 最近、且在範圍內的掉落物。
    /// 回傳是否有成功撿起。
    /// </summary>
    public static bool TryPickupNearest(Vector3 position)
    {
        WorldItemPickup nearest = null;
        float bestDist = float.MaxValue;

        for (int i = s_inRange.Count - 1; i >= 0; i--)
        {
            var p = s_inRange[i];
            if (p == null) { s_inRange.RemoveAt(i); continue; }

            float d = (p.transform.position - position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                nearest = p;
            }
        }

        return nearest != null && nearest.TryPickup();
    }

    private bool TryPickup()
    {
        if (itemData == null) return false;
        if (BackpackManager.Instance == null) return false;

        var spawned = BackpackManager.Instance.SpawnItem(itemData);
        if (spawned == null) return false; // 背包滿了,留在地上

        s_inRange.Remove(this);
        Destroy(gameObject);
        return true;
    }
}
