using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryManager : MonoBehaviour
{
    [Header("Battery Settings")]
    public Image battery;
    public float loseBatteryRate = 2.0f;
    public bool loseBattery = false;
    public bool batteryAlive = true;
    
    void Update()
    {
        if (battery.fillAmount >= 0)
        {
            batteryAlive = true;
        }

        if (battery.fillAmount <= 0)
        {
            batteryAlive = false;
        }

        if (loseBattery && batteryAlive)
        {
            battery.fillAmount -= 1.0f/loseBatteryRate * Time.deltaTime;
        }        
    }
}
