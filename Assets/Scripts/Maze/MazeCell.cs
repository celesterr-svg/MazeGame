using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public GameObject northWall, southWall, eastWall, westWall;

    [HideInInspector]
    public bool visited;

    public void resetWalls()
    {
        visited = false;
        northWall.SetActive(true);
        southWall.SetActive(true);
        eastWall.SetActive(true);
        westWall.SetActive(true);
    }

    public void Open(string direction) 
    {
        switch (direction)
        {
            case "N": northWall.SetActive(false);  
                break;
            case "S": southWall.SetActive(false);
                break;
            case "E": eastWall.SetActive(false);
                break;
            case "W": westWall.SetActive(false);
                break;
        }
    }
}
