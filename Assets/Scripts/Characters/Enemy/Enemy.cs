using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    private void Awake()
    {
        foreach(var component in GetComponents<IEnemyComponent>())
        {
            component.Initialize(data);
        }
    }

    void OnEnable()
    {
        EnemyManager.Instance.Register(this);
    }
    void OnDisable()
    {
        EnemyManager.Instance.Remove(this);
    }
}
