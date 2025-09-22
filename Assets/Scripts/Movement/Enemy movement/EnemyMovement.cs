using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chase settings")]
    public Transform goal;
    public bool chase = false;

    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (chase)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = goal.position;
        }
    }
}
