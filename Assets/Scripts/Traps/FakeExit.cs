using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.gameOver = true;            
        }
    }
}
