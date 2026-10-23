using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Scriptable Objects/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;

    public GameObject projectilePrefab;

    public float damage;

    public float cooldown;

    public float hitInterval;   // 持續型武器對同一目標的傷害間隔

    public float spreadAngle;   // 散射武器的扇形總角度(度)

    public float speed;

    public float lifetime;

    public int amount;

    public float range;

    public int maxLevel;

    public GameObject weaponPrefab;
}
