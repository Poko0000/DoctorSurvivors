using UnityEngine;

/// <summary>聖水:每次攻擊在範圍內隨機敵人的腳下生成 amount 個傷害區域,持續 lifetime 秒</summary>
public class HolyWaterWeapon : IWeapon
{
    protected override void Attack()
    {
        int count = Mathf.Max(1, data.amount);

        for (int i = 0; i < count; i++)
        {
            // range = 索敵範圍(以玩家為中心)
            Enemy target = EnemyManager.Instance.GetRandomEnemyInRange(transform.position, data.range);

            if (target == null) return;   // 範圍內已經沒有敵人,不用再抽

            SpawnZone(target.transform.position);
        }
    }

    void SpawnZone(Vector3 position)
    {
        // 區域留在原地,不跟隨敵人移動
        GameObject zone = Instantiate(data.projectilePrefab, position, Quaternion.identity);

        zone.GetComponent<DamageZone>().Initialize(data.damage, data.hitInterval, data.lifetime);
    }
}
