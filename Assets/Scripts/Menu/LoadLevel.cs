using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    public Button boton;
    public string sceneName;

    void Start()
    {
        boton = gameObject.GetComponent<Button>();
        boton.onClick.AddListener(LoadScene);
    }
    void LoadScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
