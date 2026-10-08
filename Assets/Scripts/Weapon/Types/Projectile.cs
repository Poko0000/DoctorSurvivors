using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 direction;
    float damage;
    float speed;
    bool hasHit;

    public void Initialize(Vector2 dir, float damage, float speed, float lifetime)
    {
        this.direction = dir;
        this.damage = damage;
        this.speed = speed;
        hasHit = false;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy 要到幀尾才生效,避免同一幀打中多隻敵人;已死亡(尚未回收)的敵人不吃子彈
        if (hasHit) return;

        if(other.TryGetComponent(out EnemyHealth enemyHealth) && !enemyHealth.IsDead)
        {
            hasHit = true;
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }    
    }
}
