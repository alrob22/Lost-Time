using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutToScene : MonoBehaviour
{
    public string sceneName;

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
