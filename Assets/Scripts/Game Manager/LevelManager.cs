using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    [Header("Botones")]
    public Button botonNivel2;
    public Button botonNivel3;

    public static class levelmanager
    {
        static public bool level2;
        static public bool level3;
    }
    private void Update()
    {
        if (levelmanager.level2)
        {
            botonNivel2.interactable = true;
        }

        if (levelmanager.level3) 
        {
            botonNivel3.interactable = true;
        }
    }

}
