using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private Transform camTrans;
    public int planeNum;
    private Vector3 camLeftBound;
    private Vector3 camRightBound;
    public float moveSpeed = 5f; // Speed of movement
    private Vector3 forward;
    private Vector3 right;
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

        // create left and right camera bounds
        if (planeNum == 1)
        {
            camLeftBound = new Vector3(-14.7f, camTrans.position.y, camTrans.position.z);
            camRightBound = new Vector3(13.4f, camTrans.position.y, camTrans.position.z);
        }

        // Determine which axes to lock for camera
        bool lockX = camLeftBound.x == camRightBound.x;
        Debug.Log("lockX: " + lockX);
        bool lockY = camLeftBound.y == camRightBound.y;
        Debug.Log("lockY: " + lockY);
        bool lockZ = camLeftBound.z == camRightBound.z;
        Debug.Log("lockZ: " + lockZ);

        // Move the player and camera only if there is input
        if (movement.magnitude > 0)
        {
            // player
            rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);

            // Rotate the player to face the movement direction
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

            // camera
            Vector3 newCamPosition = camTrans.position + movement * moveSpeed * Time.deltaTime * speedy;

            // Only move the camera if the player is within bounds
            if (!lockX)
            {
                if (transform.position.x >= camLeftBound.x && transform.position.x <= camRightBound.x)
                {
                    newCamPosition.x = Mathf.Clamp(newCamPosition.x, camLeftBound.x, camRightBound.x);
                } else
                {
                    newCamPosition.x = camTrans.position.x; // Don't allow the camera to move if player is out of bounds
                }
            } else
            {
                newCamPosition.x = camTrans.position.x; // Lock X-axis if specified
            }

            if (!lockY)
            {
                if (transform.position.y >= camLeftBound.y && transform.position.y <= camRightBound.y)
                {
                    newCamPosition.y = Mathf.Clamp(newCamPosition.y, camLeftBound.y, camRightBound.y);
                } else
                {
                    newCamPosition.y = camTrans.position.y; // Don't allow the camera to move if player is out of bounds
                }
            } else
            {
                newCamPosition.y = camTrans.position.y; // Lock Y-axis if specified
            }

            if (!lockZ)
            {
                if (transform.position.z >= camLeftBound.z && transform.position.z <= camRightBound.z)
                {
                    newCamPosition.z = Mathf.Clamp(newCamPosition.z, camLeftBound.z, camRightBound.z);
                } else
                {
                    newCamPosition.z = camTrans.position.z; // Don't allow the camera to move if player is out of bounds
                }
            } else
            {
                newCamPosition.z = camTrans.position.z; // Lock Z-axis if specified
            }

            camTrans.position = newCamPosition;
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }
}
