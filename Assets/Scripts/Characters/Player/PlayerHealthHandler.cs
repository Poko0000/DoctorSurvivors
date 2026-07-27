using System;
using System.Collections;
using UnityEngine;

public class PlayerHealthHandler : MonoBehaviour
{
    private int maxHP;
    private float invincibleTime = 1f;

    public int CurrentHP { get; private set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDead;

    private bool isInvincible;

    public void Initialize(int playerHP)
    {
        maxHP = playerHP;
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (CurrentHP == 0) return;

        CurrentHP -= damage;

        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log("player take damage. current HP = " + CurrentHP);

        StartCoroutine(InvincibleCoroutine());

        //OnHealthChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP == 0)
        {
            OnDead?.Invoke();
            Debug.Log("player die");
        }
    }

    public void Heal(int amount)
    {
        //CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);

        //OnHealthChanged?.Invoke(CurrentHP, maxHP);
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
