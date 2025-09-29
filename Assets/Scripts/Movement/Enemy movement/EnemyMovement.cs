using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chase settings")]
    public Transform goal;
    public bool chase = false;
    public Transform[] nodes;
    public bool travel = true;

    private Transform destiny;    
   
    void Update()
    {
        if (chase)
        {
            chasePlayer();

        } else if(travel)
        {
            travelTo();
        }

        if (!travel)
        {
            checkIfArrived();
        }
    }

    void chasePlayer()
    {
        NavMeshAgent monster = GetComponent<NavMeshAgent>();
        monster.destination = goal.position;
    }

    void travelTo()
    {
        travel = false;
        int i = UnityEngine.Random.Range(0, nodes.Length);
        NavMeshAgent monster = GetComponent<NavMeshAgent>();
        monster.destination = nodes[i].position;
        destiny = nodes[i];
    }

    void checkIfArrived()
    {
        Vector2 current = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.z);
        Vector2 target = new Vector2(destiny.position.x, destiny.position.z);
       
        if (current == target)
        {
            travel = true;
        }
    }
}
