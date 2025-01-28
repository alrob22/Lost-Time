using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    private Transform player;
    public Transform newPlayerPos;

    void Start()
    {
        this.player = GameObject.FindWithTag("Player").transform;
    }

    public void CutToShot()
    {
        Camera.main.transform.localPosition = transform.position;
        Camera.main.transform.localRotation = transform.rotation;

        player.localPosition = newPlayerPos.position;
        player.localRotation = newPlayerPos.rotation;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            CutToShot();
        }
    }
}