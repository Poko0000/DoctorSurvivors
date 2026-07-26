using UnityEngine;

public class TestWeapon : IWeapon
{
    protected override void Attack()
    {
        //Enemy target = EnemyManager.Instance.GetNearestEnemy(transform.position);

        //if (target == null) return;

        GameObject bullet = Instantiate(data.projectilePrefab, transform.position, data.projectilePrefab.transform.rotation);

        Projectile projectile = bullet.GetComponent<Projectile>();

        //projectile.Initialize(target.transform,data.damage,data.speed,data.lifetime);
        projectile.Initialize(transform, data.damage, data.speed, data.lifetime);

        Debug.Log(name + " is attack");
    }
}
