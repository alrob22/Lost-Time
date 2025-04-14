using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Melio : MonoBehaviour
{
    private bool overworldAnimTrigger = false;
    private NPCNavigator navigator;

    void Start()
    {
        navigator = GetComponent<NPCNavigator>();
    }

    void Update()
    {
        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();
        // Retrieve the index of the scene in the project's build settings.
        // Using build index so we can easily swap out test scenes to finished scenes, without needing to update this file.
        int buildIndex = currentScene.buildIndex;

        //Debug.Log("buildIndex = " + buildIndex);

        switch (buildIndex)
        {
            case 1:
                OverworldAnimation();
                break;
            //case 1:
            //    // Do something...
            //    break;
        }
    }

    void OverworldAnimation()
    {
        float targetTime = 13.5f * 3600;

        if (!overworldAnimTrigger && GameManager.Instance.inGameTime >= targetTime)
        {
            Debug.Log("Start Melio overworld animation :D");

            overworldAnimTrigger = true;
            // Activate the NPC's body.
            transform.GetChild(0).gameObject.SetActive(true);

            if (navigator != null)
            {
                navigator.Move();
            }
        }
    }
}
