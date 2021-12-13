using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilFunctions : MonoBehaviour
{
    /* 
    * This class contains a selection of utility functions used for various AI agents
    */

    public static Bounds getBoundingBox(GameObject[] objectList) 
    {
        Bounds b = new Bounds(objectList[0].transform.position, Vector3.zero);
        float buff = 1f;

        foreach(GameObject gmObj in objectList) 
        {
            Renderer r = gmObj.GetComponent<Renderer>();
            b.Encapsulate(r.bounds);
        }
        // reduce nav bounds by 1f as a buffer, but expand the y to contain gell patches
        Vector3 new_bMIN = new Vector3(b.min.x + buff, b.min.y - buff, b.min.z + buff);
        Vector3 new_bMAX = new Vector3(b.max.x - buff, b.max.y + buff, b.max.z - buff);
        b.SetMinMax(new_bMIN, new_bMAX);

        return b;
    }

    public static bool isPlayerInRange(Vector3 position, float radius) 
    {
        return getObjectWithTagInRange(position, radius, "Player");
    }

    public static GameObject getObjectWithTagInRange(Vector3 position, float radius, string tag) 
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == tag) 
            { 
                return hitCollider.gameObject;
            }
        }
        return null;
    }

    public static Vector3 getNearestGellPatch(Vector3 position, float radius) 
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.tag == "OrangeGell" || hitCollider.tag == "BlueGell") 
            { 
                return hitCollider.gameObject.transform.position;
            }
        }
        return Vector3.zero;
    } 

    
    public static Vector3 getPlayerFacingDirection(Vector3 position, float radius) 
    {
        return getObjectWithTagInRange(position, radius, "Player").transform.forward;
    } 

    public static Vector3 getPlayerposition() 
    {
        return GameObject.Find("Player").transform.position;
        
    } 

    public static Vector3 constrictPointToNavBound(Bounds navbound, Vector3 point) 
    {
        if (navbound.Contains(point)) 
        {   // if point is within bound
            return point;
        } else {
            // set point to be within nav bounds edges
            return navbound.ClosestPoint(point); 
        }
    }

    public static float getPlayerDistanceToFinish() 
    {
        GameObject player = GameObject.Find("Player");
        Vector3 finish = getFinishPosition();
        return Vector3.Distance(player.transform.position, finish);
    }

    public static Vector3 getFinishPosition() 
    {
        return  GameObject.Find("Finish").transform.position;
    }
}
