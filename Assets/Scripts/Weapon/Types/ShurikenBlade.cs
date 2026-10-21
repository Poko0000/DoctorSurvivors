using System.Collections.Generic;
using UnityEngine;

public class ShurikenBlade : MonoBehaviour
{
    float damage;
    float hitInterval;
    readonly Dictionary<EnemyHealth, float> nextHitTime = new();

    public void Initialize(float damage, float hitInterval) 
    { 
        this.damage = damage;
        this.hitInterval = hitInterval; 
    }

    // 每次刀刃重新出現都清掉紀錄,避免物件池舊敵人的 key 累積
    void OnDisable()
    {
        nextHitTime.Clear();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out EnemyHealth enemy) || enemy.IsDead) return;

        if (nextHitTime.TryGetValue(enemy, out float next) && Time.time < next) return;

        enemy.TakeDamage(damage);
        Debug.Log("造成"+damage);
        nextHitTime[enemy] = Time.time + hitInterval;
    }
}
