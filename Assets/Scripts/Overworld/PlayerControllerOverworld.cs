using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOverworld : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float sprintTransitSpeed = 5f;
    public float turnSpeed = 2f;
    public float gravity = 9.81f;

    private float speed;
    private float verticalVelocity;

    [Header("Input")]
    private float verticalMoveInput;
    private float horizontalMoveInput;
    private float turnInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        InputManagement();
        Movement();
    }

    void InputManagement()
    {
        // Get input from the player
        verticalMoveInput = Input.GetAxis("Vertical");
        horizontalMoveInput = Input.GetAxis("Horizontal");
        turnInput = 0f;

        // Handle turn input for Q (left) and E (right)
        if (Input.GetKey(KeyCode.Q))
        {
            turnInput = -1f; // Rotate left
        }
        else if (Input.GetKey(KeyCode.E))
        {
            turnInput = 1f; // Rotate right
        }
    }

    void Movement()
    {
        GroundMovement();
        Turn();
    }

    void GroundMovement()
    {
        Vector3 move = new Vector3(horizontalMoveInput, 0, verticalMoveInput);
        move = transform.TransformDirection(move);

        if (Input.GetKey(KeyCode.Tab))
        {
            speed = Mathf.Lerp(speed, sprintSpeed, sprintTransitSpeed * Time.deltaTime);
        } else
        {
            speed = Mathf.Lerp(speed, walkSpeed, sprintTransitSpeed * Time.deltaTime);
        }

        move *= speed;
        move.y = VerticalForceCalculation();

        controller.Move(move * Time.deltaTime);
    }

    void Turn()
    {
        if (turnInput != 0)
        {
            // Rotate the player around the Y-axis
            transform.Rotate(Vector3.up * turnInput * turnSpeed);
        }
    }

    float VerticalForceCalculation()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
        } else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        return verticalVelocity;
    }
}