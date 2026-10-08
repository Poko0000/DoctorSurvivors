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
        // 場景卸載時 EnemyManager 可能已經先被銷毀
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.Remove(this);
    }

    public void Init(EnemyData data)
    {
        // 物件池用 Data 當 Key 回收,必須跟生成時用的資料一致
        Data = data;

        foreach(var component in GetComponents<IEnemyComponent>())
        {
            component.Initialize(data);
        }
    }
}
