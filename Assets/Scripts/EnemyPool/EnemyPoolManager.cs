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
    }

    private void Start()
    {
        // 預熱放在 Start:生成的敵人會在 Awake/OnEnable 用到 EnemyManager、PlayerController,
        // 要等所有物件的 Awake 都跑完才安全
        foreach (EnemyData data in enemyDatas)
        {
            if (!pools.ContainsKey(data))
                pools.Add(data, new EnemyPool(data));
        }     
    }

    public Enemy Get(EnemyData data)
    {
        if (!pools.TryGetValue(data, out EnemyPool pool))
        {
            // 沒在 enemyDatas 裡登記的敵人,第一次用到時再建池子
            pool = new EnemyPool(data);
            pools.Add(data, pool);
        }
        return pool.Get();
    }

    public void Return(Enemy enemy)
    {
        if (pools.TryGetValue(enemy.Data, out EnemyPool pool))
            pool.Return(enemy);
        else
            Destroy(enemy.gameObject);
    }
}
