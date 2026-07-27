using UnityEngine;

public class Projectile : MonoBehaviour
{
    Transform target;

    float damage;
    float speed;

    public void Initialize(Transform target, float damage, float speed, float lifetime)
    {
        this.target = target;
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

        transform.position += target.transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Enemy enemy = other.GetComponent<Enemy>();

        //if (enemy == null) return;

        //enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
