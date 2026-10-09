using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthHandler : MonoBehaviour
{
    private int maxHP;
    private float invincibleTime = 1f;

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;

    public event Action<int, int> OnHealthChanged;   // (目前血量, 最大血量),給 HUD 訂閱
    public event Action OnDead;

    private bool isInvincible;

    public void Initialize(int playerHP)
    {
        maxHP = playerHP;
        CurrentHP = maxHP;
        // 初始化也廣播一次:不管 HUD 比玩家早或晚 Start,都能拿到正確數值
        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (CurrentHP == 0) return;

        CurrentHP -= damage;

        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log("player take damage. current HP = " + CurrentHP);

        StartCoroutine(InvincibleCoroutine());

        OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP == 0)
        {
            OnDead?.Invoke();
            Debug.Log("player die");
        }
    }

    public void Heal(int amount)
    {
        if (CurrentHP == 0) return;   // 死了就不能補血
        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);

        OnHealthChanged?.Invoke(CurrentHP, maxHP);
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }
}
