using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private float spawnOffset = 3f;
    private float[] spawnTimers;
    private WaveData currentWave;
    private bool hasLoggedMissingPlayer;
    private Transform player
    {
       
        get 
        {
            
            if (PlayerController.Instance == null)
            {
                if (!hasLoggedMissingPlayer)
                {
                    Debug.LogError("[Spawner] 找不到 PlayerController.Instance,敵人不會生成。");
                    hasLoggedMissingPlayer = true;
                }
                return null;
            }
            // 找到了就把旗標重設,之後如果又遺失還能再報一次
            hasLoggedMissingPlayer = false;
            return PlayerController.Instance.transform;
        }
    }


    private void Update()
    {
        currentWave = WaveManager.Instance.CurrentWave;

        if (currentWave == null) return;
        if(currentWave.endTime < WaveManager.Instance.GameTime) return;

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

        // 先確認玩家存在,再從物件池取敵人
        Transform target = player;
        if (target == null) return;

        //限制同種敵人數量
        if (EnemyManager.Instance.GetAliveCount(info.enemyData) >= info.maxAlive) return;

        Enemy enemy = EnemyPoolManager.Instance.Get(info.enemyData);
        enemy.transform.position = GetSpawnPosition(target.position);

        // 註冊由 Enemy.OnEnable 自動處理,這裡不要再 Register,不然會重複登記
        enemy.Init(info.enemyData);
    }

    Vector3 GetSpawnPosition(Vector3 center)
    {
        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0:
                return center + new Vector3(-width - spawnOffset, Random.Range(-height, height));

            case 1:
                return center + new Vector3(width + spawnOffset, Random.Range(-height, height));

            case 2:
                return center + new Vector3(Random.Range(-width, width), height + spawnOffset);

            default:
                return center + new Vector3(Random.Range(-width, width), -height - spawnOffset);
        }
    }
}
