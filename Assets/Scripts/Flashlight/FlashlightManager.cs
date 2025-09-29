using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    public Light Light;
    public BatteryManager battery;

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && battery.batteryAlive)
        {
            Light.enabled = !Light.enabled;
            battery.loseBattery = !battery.loseBattery;
        }

        if (!battery.batteryAlive)
        {
            Light.enabled = false;
        }
    }
}
