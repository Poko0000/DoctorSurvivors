using UnityEngine;

public class EnemyHealth : MonoBehaviour, IEnemyComponent
{
    private EnemyData data;

    private float hp;
    private bool isDead;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        hp = data.maxHealth;
        isDead = false;
    }

    public bool IsDead => isDead;

    public void TakeDamage(float damage)
    {
        // 同一幀被多顆子彈打到時,避免重複死亡(重複掉落、重複回收進物件池)
        if (isDead) return;

        hp -= damage;
        //Debug.Log("enemy take "+ damage + " damage");
        if (hp <= 0)
        {
            Die();   
        }
    }

    private void Die()
    {
        isDead = true;
        GetComponent<EnemyDrop>().Drop();
        //Destroy(gameObject);
        EnemyPoolManager.Instance.Return(GetComponent<Enemy>());
    }
}
