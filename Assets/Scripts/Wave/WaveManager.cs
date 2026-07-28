using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves;


    public WaveData GetCurrentWave(float time)
    {
        WaveData result = null;


        foreach(var wave in waves)
        {
            if(time >= wave.startTime)
                result = wave;
        }


        return result;
    }
}
