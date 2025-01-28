using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotZone : MonoBehaviour
{
    public Shot targetShot;
    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            targetShot.CutToShot();

            // Get the ShotTracker component from the camera
            ShotTracker shotTracker = Camera.main.GetComponent<ShotTracker>();

            if (shotTracker != null)
            {
                // Set the prevShot to the targetShot
                shotTracker.SetPrevShot(targetShot);
            }
        }
    }
}