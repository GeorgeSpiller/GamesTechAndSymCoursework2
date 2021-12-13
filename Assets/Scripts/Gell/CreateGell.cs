using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGell : MonoBehaviour
{

    public float gellPatchSize;
    public Vector3 scale;
    public float patchMergeDistance = 1f;
    public float maxPatchSize = 40f;
    public GameObject GridHandeler;

    public GameObject gellSplatterReff;
    public GameObject gellBlobReff;
    public Material OrangeGellMat;
    public Material BlueGellMat;

    private bool interactWithGrid = false;

    void Start()
    {
        scale = new Vector3(gellPatchSize, 0.2f, gellPatchSize);
        GameObject gridHandlerFind = GameObject.Find("AI-AStar");
        if (gridHandlerFind) 
        {
            GridHandeler = gridHandlerFind;
            interactWithGrid = true;
        } else {
            GridHandeler = new GameObject();
        }
    }
    public GameObject createNewGellPatch(string gellType, Vector3 position) 
    {
        position = new Vector3(position.x, position.y + 0.5f, position.z);
        
        if (interactWithGrid) 
        {
            GridHandeler.GetComponent<HandelGrid>().removeNodeNear(position, Vector3.zero);
        }

        // handels the creation of a gell patch, setting its type and material appropriatly
        GameObject newGellSplatter = Instantiate(gellSplatterReff, position, Quaternion.identity);
        MeshRenderer meshRenderer = newGellSplatter.GetComponent<MeshRenderer>();

        meshRenderer.material = gellType == "OrangeGell" ? OrangeGellMat : BlueGellMat;
        newGellSplatter.tag = gellType == "OrangeGell" ? "OrangeGell" : "BlueGell";
        
        // return the object as it may be merged/destroyed depending on surrounding patches
        return newGellSplatter;
    }

    public void formatIntercetingPatches(GameObject currentPatch) {
        /*
        This function handels gell patch interactions with eachother.
        */
        
        // We need to have access to all nearby patches, inorder to calculate weather we need to merge or
        // destroy the new patch thats trying to be created
        Collider[] hitColliders = Physics.OverlapSphere(currentPatch.transform.position, patchMergeDistance);
        GameObject tmpGellPatch = currentPatch;

        // for each nerby patch
        foreach (var hitCollider in hitColliders)
        {
            if (currentPatch.tag == hitCollider.gameObject.tag) 
            { // we only care about other gell patches that are of the same type
                if (hitCollider.gameObject != currentPatch) 
                { // if its a new patch
                    // streech the current patch to merge with the nearby patches
                    streachIntersectingGellPatches(currentPatch, hitCollider.gameObject);

                    if (tmpGellPatch != hitCollider.gameObject && tmpGellPatch != currentPatch) 
                    { // there are 3 patches colliding, remove current patch (thats trying to be created)
                        destroyGellPatch(currentPatch);
                    } else 
                    {   // this is used to count if we have three intersecting patches
                        tmpGellPatch = hitCollider.gameObject;
                    }
                }
            } else 
            {   // if the two patches are opposite colors, destroy thee patch that its colliding with.
                // this allows for the player to correct any missplaced gell patch
                string oppositePatchType = currentPatch.tag == "OrangeGell" ? "BlueGell" : "OrangeGell";
                if (hitCollider.gameObject.tag == oppositePatchType) 
                {
                    destroyGellPatch(hitCollider.gameObject);
                }

            }
        }
    }

    private void streachIntersectingGellPatches(GameObject patch1, GameObject patch2)
    {
        /*
            Handels streaching a patch to encompas another patch, then removing the smaller patch.
            Both helps woth efficency and demonstrates changing hit colliders.
        */

        // calculate the new position of where the streched patch needs to be placed. This position is
        // in the middle of the two patches, but at the same height (as both the patches must iether be
        // resting on the floor, or above the floor meaning they will eventially fall down to rest on 
        // floor)
        Vector3 newPatchPosition = new Vector3( 
            (patch1.transform.position.x + patch2.transform.position.x) / 2, 
            patch1.transform.position.y,  
            (patch1.transform.position.z + patch2.transform.position.z) / 2 );

        // Calculate the vector that is the difference in positions between two patches
        Vector3 pointTowardsOtherPatch = patch2.transform.position - patch1.transform.position;
        // returns the vector but with x, y, z as guarenteed positive values
        // used as we always want to scale patchs UP (make bigger).
        Vector3 newPatchScale = Vector3ToPositive(patch1.transform.localScale + pointTowardsOtherPatch);

        // only strech if its within bounds of a max patch size.
        if (newPatchScale.sqrMagnitude < maxPatchSize) 
        {
            // only strech if the patch scale increases
            float patch1Mag = patch1.transform.localScale.sqrMagnitude;
            float patch2Mag = patch2.transform.localScale.sqrMagnitude;
            if (newPatchScale.sqrMagnitude > patch1Mag && newPatchScale.sqrMagnitude > patch2Mag) 
            {
                // All prerequisites for creating a streched patch have been met. Scale up the third 'temporary'
                // patch we have created, destroy the two parameter patches, and instancate our 'temporary' patch
                newPatchScale += patch1.transform.localScale;

                // destroy both patches, create new patch
                destroyGellPatch(patch1);
                destroyGellPatch(patch2);

                GameObject newGellSplatter = createNewGellPatch(patch1.tag, newPatchPosition);
                newGellSplatter.transform.localScale = newPatchScale;
                if (interactWithGrid) 
                {
                    GridHandeler.GetComponent<HandelGrid>().removeNodeNear(newPatchPosition, newPatchScale);
                }
            }
        }
    }

    public void destroyGellPatch(GameObject patch) 
    {
        // we need a separate destroy function rather than the default one, as when Destroying objects we
        // need to guarentee tht it does not persist in the player list (list of patches that are currently
        // interacting with the player). This stops the player from situations that always give speed boost. 
        // remove patch form the list of currently colliding patches, and destroy it
        GetComponentInParent<GellCollision>().OrangeGellsInRange.Remove(patch);
        Destroy(patch);
    }
    
    private Vector3 Vector3ToPositive(Vector3 vector) 
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
    /*
    Code may be used in part 2 of the coursework
    // private Bounds collateManyGellPatches(GameObject g) 
    // {
    //     // taken from: https://gamedev.stackexchange.com/questions/86863/calculating-the-bounding-box-of-a-game-object-based-on-its-children/86875
    //     var b = new Bounds(g.transform.position, Vector3.zero);
    //     foreach (Renderer r in g.GetComponentsInChildren<Renderer>()) {
    //         b.Encapsulate(r.bounds);
    //     }
    //     return b;
    // }

    // private void mergeGellPatchMeshes(GameObject patch1, GameObject patch2) {
    //     // // calc new position for the new merged patch
    //     // // ( patch1.transform.position +  patch2.transform.position ) / 2;
    //     // Vector3 position = Vector3.zero;
    //     // // extract meshFilters from the two patches, place in array
    //     // MeshFilter[] meshFilters = new MeshFilter[2];
    //     // meshFilters[0] = patch1.GetComponent<MeshFilter>();
    //     // meshFilters[1] = patch2.GetComponent<MeshFilter>();

    //     // // array of instances to combined (only len 2, can be greater)
    //     // CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //     // int i = 0;
    //     // while (i < meshFilters.Length)
    //     // {
    //     //     // loop through meshFilters, assign mesh and transform to pos in combine array
    //     //     combine[i].mesh = meshFilters[i].sharedMesh;
    //     //     combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
    //     //     // deactivate (obj will be destroyed later)
    //     //     meshFilters[i].gameObject.SetActive(false);

    //     //     i++;
    //     // }
    //     // patch1.transform.GetComponent<MeshFilter>().mesh = new Mesh();
    //     // patch1.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    //     // patch1.transform.gameObject.SetActive(true);
    // }
    */
}
