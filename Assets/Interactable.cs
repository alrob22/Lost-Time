using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    float radius = 3f;
    public void Interact()
    {
        // This is where you define what happens when the player interacts with the object
        Debug.Log("Interacted with " + gameObject.name);

        // Example: Change the object's color
        GetComponent<Renderer>().material.color = Color.green;

        // You can add more logic here, like playing a sound, opening a door, etc.
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}