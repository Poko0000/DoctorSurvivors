using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveData[] waves;

    // 目前這一波開始後經過的秒數(每次換波會歸零)
    public float GameTime { get; private set; }

    public WaveData CurrentWave { get; private set; }
    private int currentWaveIndex;

    public static WaveManager Instance;

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentWaveIndex = 0;
        CurrentWave = (waves != null && waves.Length > 0) ? waves[0] : null;
    }


    private void Update()
    {
        GameTime += Time.deltaTime;

        UpdateWave();
    }

    void UpdateWave()
    {
        if (CurrentWave == null) return;

        // 用「目前這一波」的 endTime 判斷是否結束;最後一波會停在原地
        if (currentWaveIndex < waves.Length - 1 && GameTime > CurrentWave.endTime)
        {
            GameTime = 0;
            currentWaveIndex++;
            CurrentWave = waves[currentWaveIndex];
            Debug.Log("wave change");
        }
    }
}
