using System;
using UnityEngine;

public class PlayerLevelHandler : MonoBehaviour
{
    [SerializeField] int level = 1;
    [SerializeField] float exp = 0;
    [SerializeField] float levelUpExp = 100;

    public event Action OnlevelUp;

    void initLevel()
    {
        level = 1;
        exp = 0;
        levelUpExp = 100;
    }

    void LevelUp()
    {
        level++;
        exp -= levelUpExp;
        levelUpExp *= 1.1f;

        OnlevelUp?.Invoke();
    }

    public void LevelUpdate()
    {
        if(exp >= levelUpExp) LevelUp();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ExpGem expGem))
        {
            exp += expGem.exp;
            Debug.Log("player gain " + expGem.exp + " exp");
            expGem.DestroyGem();
        }
    }
}
