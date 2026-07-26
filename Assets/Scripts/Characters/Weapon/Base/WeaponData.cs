using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Weapon Data",fileName = "WeaponData_")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    public Sprite icon;

    public GameObject projectilePrefab;

    public float damage;

    public float cooldown;

    public float speed;

    public float lifetime;

    public int amount;

    public float range;

    public GameObject weaponPrefab;
}
