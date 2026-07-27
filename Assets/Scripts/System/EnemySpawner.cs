using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Enemy enemyPrefab;
    [SerializeField] float interval = 2;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= interval)
        {
            timer = 0;

            Spawn();
        }
    }

    void Spawn()
    {
        Instantiate(enemyPrefab);
    }
}
