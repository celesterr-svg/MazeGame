using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    public int health = 3;
    
    void Update()
    {
        if(health == 0)
        {
            GameManager.gameOver = true;
        }
    }
}
