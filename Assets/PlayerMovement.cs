using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    void Update()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

        // Normalize the movement vector to prevent faster diagonal movement
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Move the player
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
