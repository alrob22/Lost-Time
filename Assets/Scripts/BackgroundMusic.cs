using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    private AudioSource audioSource;
    public AudioClip overworldAudio;
    public AudioClip townAudio;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Determine desired audio clip based on scene index
        AudioClip desiredClip = scene.buildIndex < 2 ? overworldAudio : townAudio;

        // Only change the clip if it's different than the current one
        if (audioSource.clip != desiredClip)
        {
            audioSource.clip = desiredClip;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the scene loaded event when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //private void Update()
    //{
    //    Scene currentScene = SceneManager.GetActiveScene();
    //    int buildIndex = currentScene.buildIndex;



    //    if (buildIndex == 1)
    //    {
    //        audioSource.clip = townAudio;
    //        if (audioSource.isPlaying && audioSource.clip != townAudio)
    //        {
    //            audioSource.Play();
    //        }
    //    } else if (buildIndex >= 2)
    //    {
    //        audioSource.clip = townAudio;
    //        if (audioSource.isPlaying && audioSource.clip != townAudio)
    //        {
    //            audioSource.Play();
    //        }

    //    }
    //}
}
