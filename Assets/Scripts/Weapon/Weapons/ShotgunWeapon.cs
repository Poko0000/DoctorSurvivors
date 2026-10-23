using UnityEngine;

/// <summary>散彈槍:每次攻擊朝最近的敵人,以扇形一次射出 amount 顆子彈</summary>
public class ShotgunWeapon : IWeapon
{
    protected override void Attack()
    {
        Enemy target = EnemyManager.Instance.GetNearestEnemy(transform.position);

        if (target == null) return;

        Vector3 toTarget = (target.transform.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

        int count = Mathf.Max(1, data.amount);

        for (int i = 0; i < count; i++)
        {
            FirePellet(baseAngle + GetOffset(i, count));
        }
    }

    /// <summary>第 i 顆相對於中心線的偏移角度,平均分布在 [-spread/2, +spread/2]</summary>
    float GetOffset(int index, int count)
    {
        if (count == 1) return 0f;

        float step = data.spreadAngle / (count - 1);
        return -data.spreadAngle / 2f + step * index;
    }

    void FirePellet(float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        // -90:跟 TestWeapon 一致,子彈圖片預設朝上
        GameObject bullet = Instantiate(data.projectilePrefab, transform.position, Quaternion.Euler(0, 0, angleDeg - 90));

        bullet.GetComponent<Projectile>().Initialize(dir, data.damage, data.speed, data.lifetime);
    }
}
