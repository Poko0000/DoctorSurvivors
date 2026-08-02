using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveData[] waves;

    public float GameTime { get; private set; }

    public WaveData CurrentWave { get; private set; }
    private int currentWaveNum;

    public static WaveManager Instance;

    private void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentWaveNum = 1;
    }


    private void Update()
    {
        GameTime += Time.deltaTime;

        UpdateWave();
    }

    void UpdateWave()
    {
        if(currentWaveNum < waves.Length && GameTime > waves[currentWaveNum].endTime)
        {
            GameTime = 0;
            currentWaveNum++;
            Debug.Log("wave change");
        }

        CurrentWave = waves[currentWaveNum - 1];
    }
}
