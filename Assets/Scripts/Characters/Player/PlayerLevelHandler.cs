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
    public float ExpRatio => levelUpExp > 0 ? playerExp / levelUpExp : 0f;   // 避免還沒 Initialize 時除以 0

    public event Action<int> OnlevelUp;
    public event Action<float> OnExpChanged;   // 經驗條比例 0~1,給 HUD 訂閱

    public void Initialize(float baseExp, float growth)
    {
        baseLevelUpExp = baseExp;
        expGrowth = growth;

        level = 1;
        playerExp = 0;
        levelUpExp = baseLevelUpExp;
        OnExpChanged?.Invoke(ExpRatio);
    }

    private void AddExp(float amount)
    {
        playerExp += amount;

        while(levelUpExp > 0 && playerExp >= levelUpExp) 
        {
            LevelUp();
        }

        // 升級迴圈跑完才廣播,連升好幾級時 HUD 只需要更新一次
        OnExpChanged?.Invoke(ExpRatio);
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
