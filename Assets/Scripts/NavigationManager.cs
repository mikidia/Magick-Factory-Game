using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    //[SerializeField]GameObject character;

    //[SerializeField]private NavMeshAgent enemyNavMesh;
    // Start is called before the first frame update
    [SerializeField]Transform goal;

    //Reference to the NavMeshAgent
    UnityEngine.AI.NavMeshAgent agent;

    // Use this for initialization
    void Start ()
    {
        //You get a reference to the destination point inside your scene
        goal = GameObject.Find("Player").GetComponent<Transform>();

        //Here you get a reference to the NavMeshAgent
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        //You indicate to the agent to what position it has to move

        // Update is called once per frame
        //    void Update()
        //{
        //    enemyNavMesh.destination = character.transform.position;
        //}
    }
    private void FixedUpdate ()
    {
        agent.destination = goal.position;

    }
}
