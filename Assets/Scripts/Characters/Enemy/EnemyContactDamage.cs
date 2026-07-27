using UnityEngine;

public class EnemyContactDamage : MonoBehaviour, IEnemyComponent
{
    private EnemyData data;
    private int damage;
    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        damage = data.damage;
    }

    public float TakeDamage(float playerHealth)
    {
        return playerHealth - damage;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerHealthHandler playerHealth))
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
