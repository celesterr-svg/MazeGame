using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntiBattery : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 1f)] public float energyLoss;

    Image energy;
    void Start()
    {
        energy = GameObject.FindGameObjectWithTag("Energy").GetComponent<Image>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            energy.fillAmount -= energyLoss;
            Destroy(transform.parent.gameObject);
        }
    }
}
