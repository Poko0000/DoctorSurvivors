using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    List<Enemy> enemies = new();

    public void Register(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void Remove(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
