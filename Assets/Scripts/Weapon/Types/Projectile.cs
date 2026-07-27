using UnityEngine;

public class Projectile : MonoBehaviour
{
    Vector2 direction;
    float damage;
    float speed;

    public void Initialize(Vector2 dir, float damage, float speed, float lifetime)
    {
        this.direction = dir;
        this.damage = damage;
        this.speed = speed;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        //if (target == null)
        //{
            //Destroy(gameObject);
           // return;
        //}

        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }    
    }
}
