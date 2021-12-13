using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsData : MonoBehaviour
{
    public float minBound_y = -20f;
    public Vector3 levelRespawn; 

    void Start()
    {
        levelRespawn = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
    }
}
