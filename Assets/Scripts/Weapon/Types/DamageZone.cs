using System.Collections.Generic;
using UnityEngine;

/// <summary>地面傷害區域:存在 lifetime 秒,期間對區域內的敵人每隔 hitInterval 秒造成一次傷害</summary>
public class DamageZone : MonoBehaviour
{
    float damage;
    float hitInterval;
    readonly Dictionary<EnemyHealth, float> nextHitTime = new();

    public void Initialize(float damage, float hitInterval, float lifetime)
    {
        this.damage = damage;
        this.hitInterval = hitInterval;

        Destroy(gameObject, lifetime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out EnemyHealth enemy) || enemy.IsDead) return;

        if (nextHitTime.TryGetValue(enemy, out float next) && Time.time < next) return;

        enemy.TakeDamage(damage);
        nextHitTime[enemy] = Time.time + hitInterval;
    }
}
