using System;
using UnityEngine;

// 事件頻道(Observer Pattern 的 ScriptableObject 版本)
// 發送方:EnemyHealth 死亡時呼叫 RaiseEnemyDied()
// 接收方:GameFlowController 訂閱並累計擊殺數(之後成就、任務、音效也能各自訂閱)
// 好處:EnemyHealth 完全不知道誰在統計擊殺
[CreateAssetMenu(fileName = "EnemyDeathEventChannel", menuName = "Scriptable Objects/Events/EnemyDeathEventChannel")]
public class EnemyDeathEventChannel : ScriptableObject
{
    // 參數是死掉的怪的資料,之後想統計「各種怪各殺幾隻」也用得到
    public event Action<EnemyData> OnEnemyDied;

    public void RaiseEnemyDied(EnemyData data)
    {
        OnEnemyDied?.Invoke(data);
    }
}
