using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    public EnemyData[] enemyDatas;

    private Dictionary<EnemyData, EnemyPool> pools;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        pools = new Dictionary<EnemyData, EnemyPool>();
        
        foreach (EnemyData data in enemyDatas)
        {
            pools.Add(data, new EnemyPool(data));
        }     
    }

    public Enemy Get(EnemyData data)
    {
        return pools[data].Get();
    }

    public void Return(Enemy enemy)
    {
        pools[enemy.Data].Return(enemy);
    }
}
