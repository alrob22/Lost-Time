using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    private Transform player;
    private PlayerMovement playerMovement;
    public Transform newPlayerPos;
    public int newPlaneNum;

    void Start()
    {
        this.player = GameObject.FindWithTag("Player").GetComponent<Rigidbody>().transform;
        this.playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void CutToShot()
    {
        Camera.main.transform.localPosition = transform.position;
        Camera.main.transform.localRotation = transform.rotation;

        player.localPosition = newPlayerPos.position;
        player.localRotation = newPlayerPos.rotation;

        // Change the planeNum
        if (playerMovement != null)
        {
            playerMovement.planeNum = newPlaneNum;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            CutToShot();
        }
    }
}