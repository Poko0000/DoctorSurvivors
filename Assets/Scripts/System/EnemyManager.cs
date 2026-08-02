using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private Dictionary<EnemyData, List<Enemy>> enemies;

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        enemies = new Dictionary<EnemyData, List<Enemy>>();
    }

    public void Register(Enemy enemy)
    {
        if (!enemies.ContainsKey(enemy.Data))
        {
            enemies.Add(enemy.Data, new List<Enemy>());
        }

        enemies[enemy.Data].Add(enemy);
    }

    public void Remove(Enemy enemy)
    {
        if (enemies.ContainsKey(enemy.Data))
        {        
            enemies[enemy.Data].Remove(enemy);
        }
    }

    public int GetAliveCount(EnemyData data)
    {
        if (data == null)
        {
            Debug.LogError("EnemyData is null");
            return 0;
        }

        if (enemies == null)
        {
            Debug.LogError("Enemy dictionary is null");
            return 0;
        }

        if (!enemies.TryGetValue(data, out List<Enemy> list))
        {
            return 0;
        }   
        
        return enemies[data].Count;
    }

    public Enemy GetNearestEnemy(Vector3 position)
    {
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var pair in enemies)
        {
            foreach (Enemy enemy in pair.Value)
            {
                float distance = Vector3.Distance(position, enemy.transform.position);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        }

        return nearest;
    }
}
