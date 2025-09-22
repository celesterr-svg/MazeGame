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
        }

    }

    private void OnTriggerExit(Collider other) 
    { 
        if(other.gameObject.tag == "Player")
        {
            chase.chase = false;
        }
    }
}
