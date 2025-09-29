using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRestart : MonoBehaviour
{
    public MazeGeneratorSecond maze;
    public GameObject fakeExit;
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {            
            maze.Generate();
            Destroy(fakeExit);
            Destroy(gameObject);
        }
    }
}
