using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHub : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public Rigidbody rb;

    private GameManager gameManager; //so we don't move when we're talking to someone

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindFirstObjectByType<GameManager>(); //Get the GameManager
    }

    void FixedUpdate()
    {
        // Gnarly hack so we can't move while we're talking
        if (!gameManager.talking) {
            // Horizontal input moves the player along the X axis.
            // Vertical input moves the player along the Y axis.
            float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
            float verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

            // Use the input directly for world-space movement.
            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f).normalized;

            // Optionally modify speed (e.g., hold a button to move faster)
            float speedy = 1f;
            if (Input.GetAxisRaw("Cancel") == 1f)
            {
                speedy *= 3f;
            }

            if (movement.magnitude > 0)
            {
                // Move the player in world space
                rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime * speedy);

                Vector3 desiredForward = new Vector3(horizontalInput, 0f, verticalInput);
                if (desiredForward.sqrMagnitude > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(desiredForward, Vector3.up);
                    // Smoothly rotate the player to face the direction (rotating only around Y).
                    rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.deltaTime);
                }

                // Clear any residual velocities.
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
            }
        }
    }
}
