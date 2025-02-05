using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotTracker : MonoBehaviour
{
    private Transform player;
    public Shot startingShot;
    [SerializeField] private Shot prevShot;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Rigidbody>().transform;
        startingShot.CutToShot();
        prevShot = startingShot;
    }

    void Update()
    {
        // Get the player's position but keep the camera's original height (X rotation locked)
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        // Make the camera look at the player without tilting up/down
        transform.LookAt(targetPosition);
    }

    public Shot GetPrevShot()
    {
        return prevShot;
    }

    public void SetPrevShot(Shot newShot)
    {
        prevShot = newShot;
    }
}