using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    public WeaponData data;
    protected float timer;

    protected virtual void Update()
    {
        timer += Time.deltaTime;

        if (timer >= data.cooldown)
        {
            timer = 0;

            Attack();
        }
    }

    protected abstract void Attack();
}
