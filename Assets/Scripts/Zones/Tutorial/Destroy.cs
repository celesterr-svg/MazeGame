using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public MazeGeneratorSecond maze;
    public GameObject exit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (exit != null && maze != null)
            {
                Destroy(exit);
                maze.Generate();
            }
            Destroy(gameObject);
        }
    }
}
