using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTester : MonoBehaviour
{
    [SerializeField]
    public GameObject destination;
    void Update()
    {
        // drive the AI towards its assigned destination
        GetComponent<NavMeshAgent>().destination = destination.transform.position;
    }
}
