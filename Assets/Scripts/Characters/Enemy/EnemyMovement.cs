using UnityEngine;

public class EnemyMovement : MonoBehaviour, IEnemyComponent
{
    private EnemyData data;

    private Transform player;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        player = PlayerController.Instance.transform;
    }

    void Update()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        transform.position += (Vector3)dir * data.moveSpeed * Time.deltaTime;
    }
}
