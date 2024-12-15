using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyDataContainer", order = 1)]
public class EnemyDataContainer : ScriptableObject
{
    [Header("Basic Info")]
    public string name;

    [Header("Stats")]
    [Tooltip("Maximum health of the enemy.")]
    public float maxHp;

    [Tooltip("Current health of the enemy. Reset to MaxHp on spawn.")]
    public float enemyHp;

    [Tooltip("Damage dealt by the enemy.")]
    public float damage;

    [Tooltip("Movement speed of the enemy.")]
    public float moveSpeed;
    [Tooltip("Detection radius of the enemy.")]

    public float detectionRange;


    [Tooltip("Fov distance of the enemy.")]

    public float fovDistance;

    [Tooltip("Field Of view angle of the enemy.")]

    public float fieldOfViewAngle;
        [Tooltip("Attack radius of the enemy.")]

    public float attackRadius;




    // Initialize EnemyHp to MaxHp at the start.
    private void OnEnable()
    {
        enemyHp = maxHp;
    }



    // Helper method to apply damage and return if the enemy is dead
    public bool ApplyDamage(float damage)
    {
        enemyHp -= damage;
        return enemyHp <= 0;
    }
}
