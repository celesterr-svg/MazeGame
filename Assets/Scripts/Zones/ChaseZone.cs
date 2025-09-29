using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseZone : MonoBehaviour
{
    public EnemyMovement chase;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            chase.chase = true;
            chase.travel = false;
        }

    }

    private void OnTriggerExit(Collider other) 
    { 
        if(other.gameObject.tag == "Player")
        {
            chase.chase = false;
            chase.travel = true;
        }
    }
}
