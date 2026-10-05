using UnityEngine;

/// <summary>
/// 掛在「掉在地上的物品」Prefab 上(例如一個 SpriteRenderer + Collider2D)。
/// 玩家角色走近碰到它(Trigger)時,自動把這個物品撿回背包,然後這個世界物件就銷毀。
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

    private void Reset()
    {
        // 掉落物的碰撞體一定要是 Trigger,不然玩家會被它卡住、撞不過去
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (itemData == null) return;
        if (BackpackManager.Instance == null) return;

        var spawned = BackpackManager.Instance.SpawnItem(itemData);
        if (spawned != null)
        {
            // 成功放進背包才銷毀這個世界物件;如果背包滿了(spawned 是 null),
            // 就留在原地,不會憑空消失。
            Destroy(gameObject);
        }
    }
}
