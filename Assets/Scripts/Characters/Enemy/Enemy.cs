using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData Data;

    private void Awake()
    {
        Init(Data);
    }

    void OnEnable()
    {
        EnemyManager.Instance.Register(this);
    }
    void OnDisable()
    {
        EnemyManager.Instance.Remove(this);
    }

    public void Init(EnemyData data)
    {

        foreach(var component in GetComponents<IEnemyComponent>())
        {
            component.Initialize(data);
        }
    }
}
