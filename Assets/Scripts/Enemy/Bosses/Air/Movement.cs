using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float maxDistance = 5f; // Maximum distance to the player

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.LogError(
                "Player is not assigned! Please set the player reference in the inspector."
            );
        }
    }

    void Update()
    {
        if (player == null)
            return; // If the player is not assigned, do nothing

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the distance is greater than maxDistance, move towards the player
        if (distanceToPlayer > maxDistance)
        {
            navMeshAgent.isStopped = false; // Allow movement
            navMeshAgent.SetDestination(player.position);
        }
        else
        {
            // Stop moving if closer than maxDistance
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath(); // Clear the path to prevent jittering
        }

        // Ensure the agent stops when very close to the player
        if (
            navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
            && !navMeshAgent.pathPending
        )
        {
            navMeshAgent.isStopped = true;
        }
    }
}
