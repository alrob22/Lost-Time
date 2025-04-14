using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Transform camTrans;
    public int planeNum;
    public float moveSpeed = 5f; // Speed of movement
    public Rigidbody rb;

    private void Start() {
        camTrans = Camera.main.transform;
        planeNum = 1;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow
        float verticalInput = Input.GetAxis("Vertical");     // W/S or Up/Down Arrow

        Vector3 forward = camTrans.forward;
        Vector3 right = camTrans.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * verticalInput + right * horizontalInput).normalized;

        float speedy = 1f;

        if (Input.GetAxisRaw("Cancel") == 1f) {
            speedy *= 3f;
        }

        // Move the player and camera only if there is input
        if (movement.magnitude > 0)
        {
            // player
            rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);

            // Rotate the player to face the movement direction
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

            // player rigidbody
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }
}
