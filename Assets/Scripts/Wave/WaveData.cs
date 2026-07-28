using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData_", menuName = "Scriptable Objects/Wave")]
public class WaveData : ScriptableObject
{
    public List<EnemyData> enemys;

    public float startTime;

    public float spawnInterval;

    public int maxCount;
}
