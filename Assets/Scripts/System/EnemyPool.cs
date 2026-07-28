using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance {get; private set;}
    public Enemy prefab;

    Queue<Enemy> pool = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Enemy Get(Enemy enemy)
    {
        if(pool.Count > 0)
        {
            enemy = pool.Dequeue();

            enemy.gameObject.SetActive(true);

            return enemy;
        }


        return Instantiate(prefab);
    }


    public void Return(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);

        pool.Enqueue(enemy);
    }
}
