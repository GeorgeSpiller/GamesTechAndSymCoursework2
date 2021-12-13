using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandelGunFire : MonoBehaviour
{
    public bool isEnabled = true;
    public Camera playerCamera;
    public GameObject gellBlobRef;
    public float fireForce = 60;
    public string currentGellType = "OrangeGell";
    public Material OrangeGellMat;
    public Material BlueGellMat;

    private bool isPrimaryGell = true;


    Vector3 getForceVector() 
    {
        // retrurn force vector that denotes the force and direction an gell blob should be fired
        Vector3 moveDir = playerCamera.transform.forward;
        return moveDir * fireForce;
    }

    void fireProjectile () 
    {
        // create blob gameObject to fire
        GameObject newGellBlob = Instantiate(gellBlobRef, transform.position, Quaternion.identity);
        Rigidbody rb = newGellBlob.GetComponent<Rigidbody>();
        // need to enable physics properties as it instanciates the refference to the gell object in the scene,
        // which has physics dissabled
        rb.isKinematic = false;
        // apply force to blob, as normal force as its initial velocity is 0 and we want smooth acceleration
        rb.AddForce(getForceVector(), ForceMode.Force);
    }

    void Start()
    {
        // default to orange, can be changed through right click
        currentGellType = "OrangeGell";
    }

    void Update()
    {
        // this bool stops the layer from being able to fire the gun during the pause screen
        if (isEnabled) 
        {
            // on primary fire (left click) fire a gell blob projectile w/ newtonian physics
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                fireProjectile();
            }

            // on secondary fire (right click) toggle the gell blob type that would be created
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                // toggle gell type and change material for the gun
                isPrimaryGell = !isPrimaryGell;
                currentGellType = isPrimaryGell ? "OrangeGell" : "BlueGell";
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.material = currentGellType == "OrangeGell" ? OrangeGellMat : BlueGellMat;
            }
        }
    }
}
