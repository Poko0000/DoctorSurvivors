using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private float spawnOffset = 3f;
    private Transform player => PlayerController.Instance.transform;
    private float[] spawnTimers;
    private WaveData currentWave;
    
    private void Update()
    {
        currentWave = WaveManager.Instance.CurrentWave;

        if (currentWave == null) return;

        if (spawnTimers == null || spawnTimers.Length != currentWave.spawnInfos.Length)
        {
            spawnTimers = new float[currentWave.spawnInfos.Length];
        }

        for (int i = 0; i < currentWave.spawnInfos.Length; i++)
        {
            SpawnInfo info = currentWave.spawnInfos[i];

            spawnTimers[i] += Time.deltaTime;

            float interval = 1f / info.spawnRate;

            if (spawnTimers[i] >= interval)
            {
                spawnTimers[i] = 0;

                SpawnEnemy(info);
            }
        }
    }

   void SpawnEnemy(SpawnInfo info)
    {
        if (info.enemyData == null)
        {
            Debug.LogError("SpawnInfo 沒有指定 EnemyData");
            return;
        }

        //限制同種敵人數量
        if (EnemyManager.Instance.GetAliveCount(info.enemyData) >= info.maxAlive) return;

        Enemy enemy = EnemyPoolManager.Instance.Get(info.enemyData);

        enemy.transform.position = GetSpawnPosition();

        enemy.Init(info.enemyData);

        EnemyManager.Instance.Register(enemy);
    }

    Vector3 GetSpawnPosition()
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                return player.position + new Vector3(-width - spawnOffset, Random.Range(-height, height));

            case 1:
                return player.position + new Vector3(width + spawnOffset, Random.Range(-height, height));

            case 2:
                return player.position + new Vector3(Random.Range(-width, width), height + spawnOffset);

            default:
                return player.position + new Vector3(Random.Range(-width, width), -height - spawnOffset);
        }
    }
}
