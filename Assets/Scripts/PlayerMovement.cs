using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = .5f; // Speed of movement
    public Transform currentCamera;
    private Rigidbody rb;
    private void Start() {
        currentCamera = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        // Get input from arrow keys or WASD
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to the camera
        Vector3 forward = currentCamera.transform.forward;
        Vector3 right = currentCamera.transform.right;

        // Ignore the camera's vertical direction
        forward.y = 0f;
        right.y = 0f;

        // Normalize directions
        forward.Normalize();
        right.Normalize();

        // Calculate the desired movement direction
        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        // Move the Rigidbody
        rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Rotate the character to face the movement direction
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }

        // Prevent unwanted angular velocity (spinning)
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
    }


    public void UpdateCamera(Transform newCamera)
    {
        currentCamera = newCamera;
    }
}
