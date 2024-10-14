using UnityEngine;
using UnityEngine.AI;


public class EnemyStateMachine : MonoBehaviour
{
    public enum EnemyState { GoToAttackPoint, Chase }
    
    public EnemyState currentState;

    public Transform player;
    public Transform[] attackPoints; // Массив с точками для атаки
    public float detectionRange = 10f;
    public float chaseSpeed = 4f;

    // NavMeshAgent для движения
    private NavMeshAgent agent;
    private Transform closestAttackPoint; // Ближайшая точка для атаки

    // Параметры FOV (Field of View)
    public float fieldOfViewAngle = 45f;
    public float fovDistance = 15f;

    void Start()
    {
        // Получаем компонент NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        // Устанавливаем скорость для NavMeshAgent
        agent.speed = chaseSpeed;

        // Переход к ближайшей точке атаки
        SetClosestAttackPoint();
        ChangeState(EnemyState.GoToAttackPoint);
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.GoToAttackPoint:
                GoToAttackPointState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
        }

        HandleStateTransitions();
        DebugInfo(); // Вызов метода для вывода отладочной информации
    }

    // Меняем состояние врага
    void ChangeState(EnemyState newState)
    {
        currentState = newState;
        Debug.Log($"State changed to: {newState}");
    }

    // Логика для перехода к ближайшей точке атаки
    void GoToAttackPointState()
    {
        if (closestAttackPoint != null)
        {
            // Устанавливаем навигацию к ближайшей точке атаки
            agent.SetDestination(closestAttackPoint.position);
        }
    }

    // Логика преследования игрока
    void ChaseState()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    // Логика переключения состояний
    void HandleStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Если игрок в поле зрения, переключаемся в состояние преследования
        if (distanceToPlayer <= fovDistance && IsPlayerInFOV())
        {
            ChangeState(EnemyState.Chase);
        }
        else if (currentState == EnemyState.Chase && distanceToPlayer > detectionRange)
        {
            // Возврат к атаке ближайшей точки, если игрок вышел за пределы зоны обнаружения
            SetClosestAttackPoint();
            ChangeState(EnemyState.GoToAttackPoint);
        }
    }

    // Определяем ближайшую точку для атаки
    void SetClosestAttackPoint()
    {
        if (attackPoints.Length == 0)
        {
            Debug.LogError("No attack points assigned!");
            return;
        }

        float shortestDistance = Mathf.Infinity;
        Transform nearestPoint = null;

        // Ищем ближайшую точку среди всех доступных точек
        foreach (Transform attackPoint in attackPoints)
        {
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
    }

    // Отладочная информация
    void DebugInfo()
    {
        Debug.Log($"Current State: {currentState}, Distance to Player: {Vector3.Distance(transform.position, player.position)}");

        // Линии до игрока и ближайшей точки атаки
        if (closestAttackPoint != null)
        {
            Debug.DrawLine(transform.position, closestAttackPoint.position, Color.green);
        }
        if (player != null)
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
        }
    }

    // Визуализация FOV и радиуса обнаружения
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        DrawFOV();
    }

    // Рисуем поле зрения (FOV)
    void DrawFOV()
    {
        Vector3 forward = transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * forward * fovDistance;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * forward * fovDistance;

        Gizmos.DrawRay(transform.position, rightLimit);
        Gizmos.DrawRay(transform.position, leftLimit);

        Gizmos.DrawLine(transform.position + rightLimit, transform.position + leftLimit);
    }

    // Проверяем, находится ли игрок в поле зрения врага
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
