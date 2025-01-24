using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public Transform currentCamera;
    private Vector3 forward;
    private Vector3 right;

    private void Start() {
        currentCamera = Camera.main.transform;

    }
    void Update()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

        Vector3 forward = currentCamera.forward;
        Vector3 right = currentCamera.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * verticalInput + right * horizontalInput).normalized;

        float speedy = 1f;


        if(Input.GetAxisRaw("Cancel") == 1f) {
            speedy *= 3f;
        }

        // Move the player only if there is input
        if (movement.magnitude > 0)
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime * speedy, Space.World);

            // Rotate the player to face the movement direction
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }

    public void UpdateCamera(Transform newCamera)
    {
        currentCamera = newCamera;
    }
}
