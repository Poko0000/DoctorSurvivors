using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemyComponent
{
    private EnemyData data;

    private float hp;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        hp = data.maxHealth;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("enemy take "+ damage + " damage");
        if (hp <= 0)
        {
            Die();   
        }
    }

    private void Die()
    {
        GetComponent<EnemyDrop>().Drop();
        Destroy(gameObject);
    }
}
