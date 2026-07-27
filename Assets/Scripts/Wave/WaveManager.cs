using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<Wave> waves;


    public Wave GetCurrentWave(float time)
    {
        Wave result = null;


        foreach(var wave in waves)
        {
            if(time >= wave.startTime)
                result = wave;
        }


        return result;
    }
}
