using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // How close the player needs to be to interact
    public KeyCode interactionKey = KeyCode.E; // Key to press for interaction

    void Update()
    {
        // Check if the player presses the interaction key
        if (Input.GetAxisRaw("Submit") == 1f)
        {
            print("played");
            TryInteract();
        }
    }

    void TryInteract()
    {
        // Perform a raycast to detect interactable objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (Collider collider in hitColliders)
        {
            // Check if the object has the Interactable script
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                // Call the Interact method on the object
                interactable.Interact();
                break; // Interact with only one object at a time
            }
        }
    }

    public float getInteractionRange() {
        return interactionRange;
    }
}