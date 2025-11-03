using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapItem : MonoBehaviour
{
    [Header("Settings")]
    public GameObject mapObject;
    public Image mapImage;
    public float duration = 5f;

    private bool used = false;
    private Renderer render;
    void Start()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map");
        mapImage = mapObject.GetComponent<Image>();
        render = gameObject.GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && used == false)
        {
            used = true;
            StartCoroutine(mapAppear());
        }
    }

    IEnumerator mapAppear()
    {
        if (mapImage != null && !mapImage.enabled)
        {
            print("Map active");
            render.enabled = false;
            mapImage.enabled = true;
            yield return new WaitForSeconds(duration);
        }

        if(mapImage != null && mapImage.enabled)
        {
            print("Map Inactive");
            mapImage.enabled = false;
            Destroy(gameObject);
        }
    }
}
