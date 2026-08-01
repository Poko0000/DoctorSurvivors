using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData_", menuName = "Scriptable Objects/Wave")]
public class WaveData : ScriptableObject
{
    [Header("時間")]
    public float startTime;
    public float endTime;

    [Header("生成設定")]
    public SpawnInfo[] spawnInfos;
}

[System.Serializable]
public class SpawnInfo
{
    public EnemyData enemy;

    [Tooltip("每秒生成幾隻")]
    public float spawnRate;

    [Tooltip("此敵人最大存在數")]
    public int maxAlive;
}
