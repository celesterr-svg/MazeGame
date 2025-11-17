using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrap : MonoBehaviour
{
    public HealthCounter health;

    private void Start()
    {
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthCounter>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {            
            health.health--;
            Destroy(gameObject);
        }
    }    
}
