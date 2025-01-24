using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionRange = 3f; // How close the player needs to be to interact
    public string interactionButton = "Submit"; // Key to press for interaction

    void Update()
    {
        // Check if the player presses the interaction button
        if (Input.GetAxisRaw(interactionButton) == 1f)
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
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