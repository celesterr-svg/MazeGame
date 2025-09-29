using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public bool gameOver = false;
    static public bool gameWon = false;
    public GameObject panelOver;
    public GameObject panelWon;

    void Update()
    {
        if (gameOver)
        {
            panelOver.SetActive(true);
            Time.timeScale = 0;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Time.timeScale = 1f;
                gameOver = false;
            }
        }

        if (gameWon)
        {
            panelWon.SetActive(true);
            Time.timeScale = 0;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Time.timeScale = 1f;
                gameWon = false;
            }
        }
    }
}
