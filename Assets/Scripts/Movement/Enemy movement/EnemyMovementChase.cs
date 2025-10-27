using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementChase : MonoBehaviour
{
    private GameObject target;
    NavMeshAgent enemy;

    void Start()
    {   
        enemy = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");        
    }

    private void Update()
    {
        enemy.destination = target.transform.position;
    }
}
