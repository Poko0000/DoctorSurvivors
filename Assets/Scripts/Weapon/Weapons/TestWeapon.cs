using UnityEngine;
using UnityEngine.UIElements;

public class TestWeapon : IWeapon
{
    protected override void Attack()
    {
        Enemy target = EnemyManager.Instance.GetNearestEnemy(transform.position);

        if (target == null) return;

        Vector3 dir = (target.transform.position - transform.position).normalized;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg)- 90;

        GameObject bullet = Instantiate(data.projectilePrefab, transform.position, Quaternion.Euler(0,0,angle));

        Projectile projectile = bullet.GetComponent<Projectile>(); 

        projectile.Initialize(dir,data.damage,data.speed,data.lifetime);

        Debug.Log(name + " is attack");
    }
}
