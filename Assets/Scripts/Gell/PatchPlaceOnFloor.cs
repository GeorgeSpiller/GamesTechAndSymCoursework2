using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchPlaceOnFloor : MonoBehaviour
{
    public float gellRestHeight = 0.1f;
    public GameObject playerGun;
    public bool isGrounded;

    void Start() 
    {
        // Used to make the patch be infulenced by gravity
        GetComponent<Rigidbody>().isKinematic = false;
    }

    void Update()
    {
        // while the patch is not on the ground, let it fall
        if (!isGrounded) {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, gellRestHeight);
            if (isGrounded) 
            {   // the moment the patch touches the ground, we can remove its interactions with the
                // physicis enviroment. Then check if this patch needs to be merged or destroyed
                GetComponent<Rigidbody>().isKinematic = true;
                playerGun.GetComponent<CreateGell>().formatIntercetingPatches(gameObject);
            }
        }
    }
}
