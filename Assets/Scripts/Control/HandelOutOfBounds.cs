using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandelOutOfBounds : MonoBehaviour
{

    public GameObject respawnPlatform;
    public bool teleportOnOOB = true;
    private float minBound_y;
    private Vector3 levelRespawn;

    void Start() 
    {
        minBound_y = respawnPlatform.GetComponent<OutOfBoundsData>().minBound_y;
        levelRespawn = respawnPlatform.GetComponent<OutOfBoundsData>().levelRespawn;
    }

    void Update() {
        if (transform.position.y < minBound_y) {
            if (teleportOnOOB) 
            {
                Vector3 tpPosition = new Vector3(levelRespawn.x, levelRespawn.y + 5f, levelRespawn.z);
                transform.position = tpPosition;
            } else 
            {
                Destroy(gameObject);
            }
        }
    }
}
