using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth;
    public float moveSpeed;
    public int damage;

    public int exp;

    public GameObject enemyPrefab;
}
