using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Scriptable Objects/Weapon Data")]
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
