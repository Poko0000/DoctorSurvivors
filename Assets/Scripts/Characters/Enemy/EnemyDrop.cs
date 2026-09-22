using UnityEngine;

public class EnemyDrop : MonoBehaviour, IEnemyComponent
{   
    private EnemyData data;

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
    }

    public void Drop()
    {
        Instantiate(data.dropPrefab,transform.position,Quaternion.identity);
    }

}
