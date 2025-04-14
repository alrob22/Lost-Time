using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;

    // Y rotation bounds
    public float leftBound = -9.119f; // Left side limit
    public float rightBound = 8.02f; // Right side limit

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Rigidbody>().transform;
    }

    void Update()
    {
        // Get the player's position but keep the camera's original height (X rotation locked)
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        // Calculate the desired rotation so that the camera would look at the player
        Quaternion desiredRotation = Quaternion.LookRotation(targetPosition - transform.position);

        // Extract the Y angle and convert from [0,360) to a signed angle (-180 to 180)
        float desiredY = desiredRotation.eulerAngles.y;
        if (desiredY > 180f)
        {
            desiredY -= 360f;
        }

        // Only update the camera's rotation if the desired Y rotation is within the specified bounds.
        if (desiredY >= leftBound && desiredY <= rightBound)
        {
            transform.rotation = desiredRotation;
        }
    }
}