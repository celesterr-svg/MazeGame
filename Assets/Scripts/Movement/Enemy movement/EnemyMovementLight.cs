using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovementLight : MonoBehaviour
{
    [Header("Settings")]
    public Image battery;

    Renderer render;
    Light lights;
    GameObject target;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        render = gameObject.GetComponent<Renderer>();
        lights = GetComponent<Light>();
    }

    void Update()
    {
        if (battery.fillAmount <= 0)
        {
            agent.destination = target.transform.position;
            render.enabled = true;   
            lights.enabled = true;
            gameObject.tag = "Monster";
        } else
        {
            gameObject.tag = "Untagged";
            render.enabled = false;
            lights.enabled = false;
            agent.destination = agent.transform.position;            
        }
    }
        

}
