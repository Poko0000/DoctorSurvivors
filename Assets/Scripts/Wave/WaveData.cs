using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData_", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("時間")]
    public float startTime;
    public float endTime;

    [Header("生成設定")]
    public SpawnInfo[] spawnInfos;
}
