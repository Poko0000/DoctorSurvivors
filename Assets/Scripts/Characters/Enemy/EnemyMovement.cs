using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour, IEnemyComponent
{
    private EnemyData data;

    private Transform player;
    private Rigidbody2D m_rigidbody;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        player = PlayerController.Instance.transform;
        if (m_rigidbody == null) m_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 用 transform 的位置當基準:從物件池取出後是直接設 transform.position,
        // 這時 rigidbody.position 可能還沒同步,用它會把敵人拉回舊位置
        Vector2 current = transform.position;
        Vector2 dir = ((Vector2)player.position - current).normalized;

        m_rigidbody.MovePosition(current + dir * data.moveSpeed * Time.fixedDeltaTime);
    }
}
