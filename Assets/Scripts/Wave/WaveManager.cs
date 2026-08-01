using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveData[] waves;

    public float GameTime { get; private set; }

    public WaveData CurrentWave { get; private set; }

    private void Update()
    {
        GameTime += Time.deltaTime;

        UpdateWave();
    }

    void UpdateWave()
    {
        foreach (var wave in waves)
        {
            if (GameTime >= wave.startTime &&
                GameTime < wave.endTime)
            {
                CurrentWave = wave;
                return;
            }
        }

        CurrentWave = waves[waves.Length - 1];
    }
}
