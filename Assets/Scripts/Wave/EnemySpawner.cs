using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float interval = 2;
    public int spawnAmount = 50;
    public float spawnRadius = 10f;
    private float timer;
    private Transform player;

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= interval)
        {
            timer = 0;

            SpawnWave();
        }
    }

    public void SpawnWave()
    {
        for(int i = 0; i < spawnAmount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        player = player = PlayerController.Instance.transform;
        Vector2 randomPos = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 pos = player.position + (Vector3)randomPos * spawnRadius;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
        EnemyPool.Instance.Get(enemyPrefab);
    }
}
