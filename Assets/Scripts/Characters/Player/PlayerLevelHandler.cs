using System;
using UnityEngine;

public class PlayerLevelHandler : MonoBehaviour
{
    private int level = 1;
    private float playerExp = 0;
    private float baseLevelUpExp;
    private float expGrowth;
    private float levelUpExp;
    //for UI
    public int Level => level;
    public float ExpRatio => playerExp / levelUpExp;

    public event Action<int> OnlevelUp;

    public void Initialize(float baseExp, float growth)
    {
        baseLevelUpExp = baseExp;
        expGrowth = growth;

        level = 1;
        playerExp = 0;
        levelUpExp = baseLevelUpExp;
    }

    private void AddExp(float amount)
    {
        playerExp += amount;

        while(levelUpExp > 0 && playerExp >= levelUpExp) 
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        playerExp -= levelUpExp;
        levelUpExp *= expGrowth;

        OnlevelUp?.Invoke(level);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ExpGem expGem))
        {
            AddExp(expGem.exp);
            expGem.DestroyGem();
        }
    }
}
