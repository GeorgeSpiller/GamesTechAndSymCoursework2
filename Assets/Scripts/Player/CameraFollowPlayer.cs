using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject targetObject;
    private Vector3 initOffset;
    private Vector3 cameraPos;
    public float glide;

    void Start()
    {  
        // constant position of the camera away from the player
        initOffset = new Vector3(0, 10, -10);
    }

    void FixedUpdate()
    {
        // main menue camera handeler, calculate next pos as player pos + offset, and
        // liniarly interpolate the cameras position to it over time (smooth movment). 
        cameraPos = targetObject.transform.position + initOffset;
        transform.position = Vector3.Lerp(transform.position, cameraPos, glide * Time.fixedDeltaTime);
        transform.LookAt(targetObject.transform);
    }
}
