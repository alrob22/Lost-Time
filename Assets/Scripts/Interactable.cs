using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    float radius = 3f;
    virtual public void Interact()
    {
        // This is where you define what happens when the player interacts with the object
        Debug.Log("Interacted with " + gameObject.name);

        // Example: Change the object's color
        GetComponent<Renderer>().material.color = Color.green;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 targetPoint = player.transform.position;
        targetPoint.y = transform.position.y;
        //targetPoint.z = transform.position.z;

        transform.LookAt(targetPoint);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}