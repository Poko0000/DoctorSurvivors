using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    [SerializeField] private Transform weaponRoot;
    public List<GameObject> weapons = new();

    public void AddWeapon(WeaponData data)
    {
        var weapon = Instantiate(data.weaponPrefab, weaponRoot);

        weapons.Add(weapon);
    }
}
