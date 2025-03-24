using UnityEngine;
using UnityEngine.AI;

public class NPCNavigator : MonoBehaviour
{
    private NavMeshAgent agent;
    // Target destination for the NPC
    public Transform targetDestination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Move()
    {
        if (agent != null && targetDestination != null)
        {
            agent.SetDestination(targetDestination.position);
        }
        else
        {
            Debug.LogError("NavMeshAgent or targetDestination not set on NPCNavigator.");
        }
    }
}
