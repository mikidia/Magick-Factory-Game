using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState { GoToAttackPoint, Chase, Attack, Patrol }

    [SerializeField] private EnemyState currentState;
    [SerializeField] private EnemyDataContainer enemyScriptableObj;

    private Transform player;
    [SerializeField] private Transform[] attackPoints;
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRadius;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private float fovDistance;

    private NavMeshAgent agent;
    private Transform closestAttackPoint;
    private Enemy enemy;

    public Transform ClosestAttackPoint { get => closestAttackPoint; set => closestAttackPoint = value; }
    public EnemyDataContainer EnemyScriptableObj { get => enemyScriptableObj; set => enemyScriptableObj = value; }

    void Awake()
    {
        InitializeFromScriptableObject();
        enemy = GetComponent<Enemy>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;
        FindObjectsToDestroy();
        if (closestAttackPoint != null)
        {
            ChangeState(EnemyState.GoToAttackPoint);
        }
        else
        {
            ChangeState(EnemyState.Patrol); // If no attack points, switch to Patrol
        }
    }

    void Update()
    {
        if (closestAttackPoint == null && attackPoints.Length > 0)
        {
            SetClosestAttackPoint();
        }

        switch (currentState)
        {
            case EnemyState.GoToAttackPoint:
                GoToAttackPointState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
            case EnemyState.Patrol:
                PatrolState();
                break;
        }

        HandleStateTransitions();
    }

    void InitializeFromScriptableObject()
    {
        if (enemyScriptableObj != null)
        {
            detectionRange = enemyScriptableObj.detectionRange;
            attackRadius = enemyScriptableObj.attackRadius;
            chaseSpeed = enemyScriptableObj.moveSpeed;
            fieldOfViewAngle = enemyScriptableObj.fovDistance;
            fovDistance = enemyScriptableObj.fovDistance;
        }
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;
        Debug.Log($"State changed to: {newState}");
    }

    void GoToAttackPointState()
    {
        if (closestAttackPoint != null)
        {
            // Move towards the closest attack point only if outside attack radius
            if (Vector3.Distance(transform.position, closestAttackPoint.position) > attackRadius)
            {
                agent.SetDestination(closestAttackPoint.position);
                FaceTarget(closestAttackPoint.position);
            }
            else
            {
                // Stop the NavMeshAgent and switch to Attack state
                agent.ResetPath();
                ChangeState(EnemyState.Attack);
            }
        }
    }

    void ChaseState()
    {
        if (player != null)
        {
            // Move towards the player only if outside attack radius
            if (Vector3.Distance(transform.position, player.position) > attackRadius)
            {
                agent.SetDestination(player.position);
                FaceTarget(player.position);
            }
            else
            {
                // Stop the NavMeshAgent and switch to Attack state
                agent.ResetPath();
                ChangeState(EnemyState.Attack);
            }
        }
    }

    void AttackState()
    {
        // Stop movement in Attack state
        agent.SetDestination(transform.position);

        if (IsBuildingInAttackRadius())
        {
            FaceTarget(closestAttackPoint.position);
            var damageable = closestAttackPoint.GetComponent<IDamageable>();
            if (damageable != null)
            {
                PerformAttack(damageable);
            }
        }
        else if (IsPlayerInAttackRadius())
        {
            FaceTarget(player.position);
            var damageable = player.GetComponent<IDamageable>();
            if (damageable != null)
            {
                PerformAttack(damageable);
            }
        }
        else
        {
            // If target exits attack radius, switch back to Chase state
            ChangeState(EnemyState.Chase);
        }
    }

    void PatrolState()
    {
        agent.SetDestination(transform.position);
    }

    void PerformAttack(IDamageable target)
    {
        target.TakeDamage(10);
        Debug.Log("Attacking target...");
    }

    void HandleStateTransitions()
    {
        float distanceToPlayer = player != null ? Vector3.Distance(transform.position, player.position) : Mathf.Infinity;

        if (IsPlayerInFOV())
        {
            ChangeState(EnemyState.Chase); // Switch to Chase if player is in FOV
        }
        else if (IsPlayerInAttackRadius())
        {
            ChangeState(EnemyState.Attack); // Switch to Attack if player within attack radius
        }
        else if (distanceToPlayer > detectionRange && closestAttackPoint != null)
        {
            ChangeState(EnemyState.GoToAttackPoint); // Go to attack point if player is out of detection range
        }
        else if (closestAttackPoint == null)
        {
            ChangeState(EnemyState.Patrol); // Enter Patrol state if no attack points
        }
    }

    void FindObjectsToDestroy()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        attackPoints = buildings.Select(b => b.transform).ToArray();
        SetClosestAttackPoint();
    }

    void SetClosestAttackPoint()
    {
        closestAttackPoint = attackPoints
            .Where(point => point != null)
            .OrderBy(point => Vector3.Distance(transform.position, point.position))
            .FirstOrDefault();

        if (closestAttackPoint == null)
        {
            Debug.LogWarning("No valid attack points remaining.");
        }
        else
        {
            Debug.Log($"Closest Attack Point found: {closestAttackPoint.name}");
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    bool IsPlayerInFOV()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        return angleToPlayer < fieldOfViewAngle / 2 && Vector3.Distance(transform.position, player.position) <= fovDistance;
    }

    bool IsPlayerInAttackRadius()
    {
        return player != null && Vector3.Distance(transform.position, player.position) <= attackRadius;
    }

    bool IsBuildingInAttackRadius()
    {
        return closestAttackPoint != null && Vector3.Distance(transform.position, closestAttackPoint.position) <= attackRadius;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (closestAttackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, closestAttackPoint.position);
            Gizmos.DrawWireSphere(closestAttackPoint.position, attackRadius);
        }

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
            DrawFOV();
        }
    }

    void DrawFOV()
    {
        Vector3 forward = transform.forward * fovDistance;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward;

        Gizmos.DrawRay(transform.position, rightLimit);
        Gizmos.DrawRay(transform.position, leftLimit);
    }
}
