using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Settings")]
    public MazeGeneratorSecond maze;
    public float playerRadius = 0.4f;
    public float yOffset = 0.1f;

    void Start()
    {
        maze = GameObject.FindGameObjectWithTag("Maze").GetComponent<MazeGeneratorSecond>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || maze == null)
        {
            return;
        }

        Vector3 destiny = maze.GetRandomSafePoint(playerRadius, 0.05f);
        destiny.y += yOffset;

        other.transform.position = destiny;
    }
}
