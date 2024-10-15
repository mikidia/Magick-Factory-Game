using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState { GoToAttackPoint, Chase, Attack }

    public EnemyState currentState;

    public Transform player;
    public Transform[] attackPoints;
    public float detectionRange = 10f;
    [SerializeField] float attackRadius = 3f; // Attack distance
    public float chaseSpeed = 4f;

    private NavMeshAgent agent;
    [SerializeField] Transform closestAttackPoint;

    public float fieldOfViewAngle = 45f;
    public float fovDistance = 15f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed;
        FindObjectsToDestroy();

        ChangeState(EnemyState.GoToAttackPoint);
    }

    void FindObjectsToDestroy()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        attackPoints = new Transform[buildings.Length];
        for (int i = 0; i < buildings.Length; i++)
        {
            attackPoints[i] = buildings[i].GetComponent<Transform>();
        }

        SetClosestAttackPoint();
    }

    void Update()
    {
        // Check if the closest attack point is missing (destroyed or disabled)
        if (closestAttackPoint == null)
        {
            Debug.Log("Closest attack point missing. Finding next one.");
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
        }

        HandleStateTransitions();
        DebugInfo();
    }

    void FixedUpdate()
    {
        SetClosestAttackPoint();
        
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
            agent.SetDestination(closestAttackPoint.position);
            FaceTarget(closestAttackPoint.position);
        }
    }

    void ChaseState()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
            FaceTarget(player.position);
        }
    }

    void AttackState()
    {
        // Stop moving and face the player
        agent.SetDestination(transform.position); // Stop movement
        FaceTarget(player.position);

        // Here, you can implement the actual attack logic (e.g., reducing player health)
        Debug.Log("Attacking player...");
    }

    void HandleStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (Vector3.Distance(transform.position,closestAttackPoint.transform.position) <= attackRadius)
        {
            ChangeState(EnemyState.Attack); // Switch to attack if player is in range
        }
        else if (distanceToPlayer <= fovDistance && IsPlayerInFOV())
        {
            ChangeState(EnemyState.Chase); // Switch to chase if player is detected
        }else if (currentState == EnemyState.Attack && Vector3.Distance(transform.position,closestAttackPoint.transform.position)  > attackRadius)
        {

            ChangeState(EnemyState.GoToAttackPoint);

        }
        // else if (Vector3.Distance(transform.position,closestAttackPoint.transform.position) > attackRadius)
        // {
        //     ChangeState(EnemyState.GoToAttackPoint);
        // }
        // else if (currentState == EnemyState.Chase && Vector3.Distance(transform.position,closestAttackPoint.transform.position)  < detectionRange)
        // {
        //     SetClosestAttackPoint();
        //     ChangeState(EnemyState.GoToAttackPoint); // Switch back to attack point if player is far
        // }
    }

    void SetClosestAttackPoint()
    {
        if (attackPoints.Length == 0)
        {
            Debug.LogError("No attack points assigned!");
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Transform nearestPoint = null;

        // Iterate through remaining valid attack points (not null)
        foreach (Transform attackPoint in attackPoints)
        {
            if (attackPoint == null) continue; // Skip destroyed attack points

            float distanceToPoint = Vector3.Distance(transform.position, attackPoint.position);
            if (distanceToPoint < shortestDistance)
            {
                shortestDistance = distanceToPoint;
                nearestPoint = attackPoint;
            }
        }

        closestAttackPoint = nearestPoint;

        if (closestAttackPoint != null)
        {
            Debug.Log($"Closest Attack Point found: {closestAttackPoint.name}");
        }
        else
        {
            Debug.LogWarning("No valid attack points remaining.");
        }
    }

    void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void DebugInfo()
    {
        Debug.Log($"Current State: {currentState},Distance to Player: {Vector3.Distance(transform.position, closestAttackPoint.position)}, Distance to Player: {Vector3.Distance(transform.position, player.position)}");

        if (closestAttackPoint != null)
        {
            Debug.DrawLine(transform.position, closestAttackPoint.position, Color.green);
        }
        if (player != null)
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack radius around the closest point of interest (either player or attack point)
        Vector3 attackCenter = (player != null && Vector3.Distance(transform.position, player.position) < Vector3.Distance(transform.position, closestAttackPoint.position))
            ? player.position
            : closestAttackPoint?.position ?? transform.position; // Default to current position if none available

        if (attackCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCenter, attackRadius); // Draw the attack radius in red
        }

        Gizmos.color = Color.red;
        DrawFOV();
    }

    void DrawFOV()
    {
        Vector3 forward = transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward * fovDistance;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward * fovDistance;

        Gizmos.DrawRay(transform.position, rightLimit);
        Gizmos.DrawRay(transform.position, leftLimit);

        Gizmos.DrawLine(transform.position + rightLimit, transform.position + leftLimit);
    }

    bool IsPlayerInFOV()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < fieldOfViewAngle / 2 && Vector3.Distance(transform.position, player.position) <= fovDistance)
        {
            return true;
        }
        return false;
    }
}
