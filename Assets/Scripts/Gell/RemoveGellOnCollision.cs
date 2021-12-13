using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveGellOnCollision : MonoBehaviour
{
    public GameObject GellGun;
    public void OnTriggerStay(Collider collider)
     {
        if (collider.gameObject.tag == "OrangeGell" || collider.gameObject.tag == "BlueGell") 
        {
            GellGun.GetComponent<CreateGell>().destroyGellPatch(collider.gameObject);
        }
    }

}
