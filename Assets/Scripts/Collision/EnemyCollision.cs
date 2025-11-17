using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && gameObject.tag == "Monster")
        {
            GameManager.gameOver = true;
        }
    }
}
