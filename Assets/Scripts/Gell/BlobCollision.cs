using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobCollision : MonoBehaviour
{

    public GameObject playerGun;
    public Material OrangeGellMat;
    public Material BlueGellMat;
    public string gellType;
    public float gellSpawnHieght = 0.5f;

    void OnCollisionEnter(Collision collision)
    {
        // If the gell blob that has been fired from the players gell gun has collided with (gellable) ground
        // gather gell contact points, remove gell blob object, start the process of spawning in gell patches.
        if (collision.gameObject.tag == "Enviroment") 
        {
            Vector3 collisionPointContact = collision.contacts[0].point;
            Vector3 patchPosition = new Vector3(collisionPointContact.x, collisionPointContact.y + gellSpawnHieght, collisionPointContact.z);
            
            // creation, merging and destruction of all gell patches are hendeled through the players gell gun.
            playerGun.GetComponent<CreateGell>().createNewGellPatch(gellType, patchPosition);
            Destroy(gameObject);
        }
    }

    void Start() 
    {
        // uppon the gell blob spawning, set its material to denote its type (orange/blue)
        gellType = playerGun.GetComponent<HandelGunFire>().currentGellType;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = gellType == "OrangeGell" ? OrangeGellMat : BlueGellMat;
    }

}
