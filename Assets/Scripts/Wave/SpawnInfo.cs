using UnityEngine;


[System.Serializable]
public class SpawnInfo
{
    public EnemyData enemyData;

    [Tooltip("每秒生成幾隻")]
    public float spawnRate;

    [Tooltip("此敵人最大存在數")]
    public int maxAlive;
}
