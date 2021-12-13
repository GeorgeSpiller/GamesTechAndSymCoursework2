using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float sensetivity = 100f;
    public bool camMovmentIsEnabled = true;

    private Camera cam;
    private float multiplier = 0.01f;
    private float mouseX;
    private float mouseY;
    private float xRot;
    private float yRot;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();

        // lock and hide the cursor for the first person camera
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Check if the player has paused, if not manage first person camera movment
        if (camMovmentIsEnabled) 
        {
            // lock and hide cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // sets X and  rotations based on mouse movment
            HandelInput();
            // translate these rotations to the camera, X directly to local space, 
            // Y to transform rotation space
            cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRot, 0);
        }        
    }

    void HandelInput() 
    {
        // get mouse inputs
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        // scale rotation values based on sensitivity values and multiplers
        xRot -= mouseY * sensetivity * multiplier;
        yRot += mouseX * sensetivity * multiplier;
        
        // clamp rottion so the player cant look too far up or down
        xRot = Mathf.Clamp(xRot, -90f, 90f);
    } 
}
