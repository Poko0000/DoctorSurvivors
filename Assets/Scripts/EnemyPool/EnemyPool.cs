using System.Collections.Generic;
using UnityEngine;

public class EnemyPool
{
    private Queue<Enemy> queue = new();

    private EnemyData data;

    public EnemyPool(EnemyData data)
    {
        this.data = data;

        for (int i = 0; i < data.prewarmCount; i++)
        {
            Enemy enemy = Object.Instantiate(data.enemyPrefab).GetComponent<Enemy>();

            Return(enemy);
        }
    }

    public Enemy Get()
    {
        Enemy enemy;

        if (queue.Count > 0)
        {
            enemy = queue.Dequeue();
        }
        else
        {
            enemy = Object.Instantiate(data.enemyPrefab).GetComponent<Enemy>();
        }
        
        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public void Return(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);

        queue.Enqueue(enemy);
    }
}
