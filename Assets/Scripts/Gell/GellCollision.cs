using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GellCollision : MonoBehaviour
{
    public List<GameObject> OrangeGellsInRange = new List<GameObject>();

     public void OnTriggerStay(Collider collider)
     {
        if (collider.gameObject.tag == "OrangeGell") 
        {
            GetComponent<PlayerMovmentPhysicsBased>().isOnSpeedGell = true;
            // add to list of currently colliding speed gell patches if it is not in already
            // this helps to 1. not stach movment forces and 2. consistantly remove speed when 
            // the player is not within a gell patch trigger  
            if (OrangeGellsInRange.IndexOf(collider.gameObject) < 0)
            {
                OrangeGellsInRange.Add(collider.gameObject);
            }

        } else if (collider.gameObject.tag == "BlueGell" && !GetComponent<PlayerMovmentPhysicsBased>().hasAppliedJump) 
        {
            // apply the imediate jump force once entered into the bule gell trigger. This applied force is clamped
            // to stop the player from flying into the air with extreme force. It is also as an impule, thus imediatly
            // applying the force, but still with respect to the players mass.
            FindObjectOfType<AudioManager>().Play("jump");
            Rigidbody rb = GetComponent<Rigidbody>();
            float jumpForce = Mathf.Clamp(rb.velocity.sqrMagnitude * GetComponent<PlayerMovmentPhysicsBased>().jumpForce, 0f, 100f);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            // used to stop multiple blue gell patch from repeatedly applying a jump force
            GetComponent<PlayerMovmentPhysicsBased>().hasAppliedJump  = true;
        }

     }

     public void OnTriggerExit(Collider collider)
     {
        // remove the gell patches from interaction lists. 
        if (collider.gameObject.tag == "OrangeGell") 
        {
            FindObjectOfType<AudioManager>().orangeFadeOut("orangegell");
            OrangeGellsInRange.Remove(collider.gameObject);
        } else if (collider.gameObject.tag == "BlueGell") 
        {
            GetComponent<PlayerMovmentPhysicsBased>().hasAppliedJump = false;
        }
     }

    void Update()
    {
        // If there are no Orange Gell patches in range, cancel speed
        if (OrangeGellsInRange.Count == 0) 
        {
            GetComponent<PlayerMovmentPhysicsBased>().isOnSpeedGell = false;
        }
    }
}
