using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave_", menuName = "Scriptable Objects/Wave")]
public class Wave : ScriptableObject
{
    public List<EnemyData> enemys;

    public float startTime;

    public float spawnInterval;

    public int maxCount;
}
