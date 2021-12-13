using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateChildPatches : MonoBehaviour
{
    public GameObject gellGun;
    void Start()
    {
        GameObject[] allChildren = new GameObject[transform.childCount];
        int i = 0;
        foreach(Transform child in transform)
        {
            gellGun.GetComponent<CreateGell>().createNewGellPatch("OrangeGell", child.position);
            allChildren[i] = child.gameObject;
            i += 1;
        }
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

    }
}
