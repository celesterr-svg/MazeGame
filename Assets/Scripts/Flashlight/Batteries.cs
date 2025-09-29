using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Batteries : MonoBehaviour
{
    [Header("Batteries Settings")]
    public Image Energy;
    [Range(0f, 1f)]public float fillAmount;

    private void Start()
    {
        Energy = GameObject.FindGameObjectWithTag("Energy").GetComponent<Image>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Energy.fillAmount += fillAmount;
            Destroy(transform.parent.gameObject);
        }
    }
}
