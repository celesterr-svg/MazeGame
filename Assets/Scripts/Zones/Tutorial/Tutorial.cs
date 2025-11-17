using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI tutorial1;
    public TextMeshProUGUI tutorial2;
    public TextMeshProUGUI tutorial3;
    public TextMeshProUGUI tutorial4;
    public TextMeshProUGUI tutorial5;
    public TextMeshProUGUI tutorial6;

    public GameObject pilatutorial;
    public GameObject trigger1;
    public GameObject trigger2;
    public GameObject trigger3;

    private bool tut1 = true;
    private bool tut2;
    private bool tut3;
    private bool tut4;
    private bool tut5;
    private bool tut6;

    private void Start()
    {
        tutorial1.GetComponent<TextMeshProUGUI>().enabled = true;
    }
    private void Update()
    {       

        if (Input.GetKeyDown(KeyCode.W) && tut1)
        {
            tutorial1.GetComponent<TextMeshProUGUI>().enabled = false;
            tutorial2.GetComponent<TextMeshProUGUI>().enabled = true;
            tut1 = false;
            tut2 = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && tut2)
        {
            tutorial2.GetComponent<TextMeshProUGUI>().enabled = false;
            tutorial3.GetComponent<TextMeshProUGUI>().enabled = true;
            tut2 = false;
            tut3 = true;
        }

        if(pilatutorial == null && tut3)
        {
            tutorial3.GetComponent<TextMeshProUGUI>().enabled = false;
            tutorial4.GetComponent<TextMeshProUGUI>().enabled = true;
            tut3 = false;
            tut4 = true;
        }

        if (trigger1 == null && tut4)
        {
            tutorial4.GetComponent<TextMeshProUGUI>().enabled = false;
            tutorial5.GetComponent<TextMeshProUGUI>().enabled = true;
            tut4 = false;
            tut5 = true;
        }

        if (trigger2 == null && tut5)
        {
            tutorial5.GetComponent<TextMeshProUGUI>().enabled = false;
            tutorial6.GetComponent<TextMeshProUGUI>().enabled = true;
            tut5 = false;
            tut6 = true;
        }

        if (trigger3 == null && tut6)
        {
            tutorial6.GetComponent<TextMeshProUGUI>().enabled = false;
            tut6 = false;
        }
    }
}
