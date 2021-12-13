using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GellableCube : MonoBehaviour
{

    public Material OrangeGellMat;
    public Material BlueGellMat;
    public GameObject player;
    public bool randomizeSize = false;
    public float maxRandomSize = 10f;
    public string currentColor = "";
    public bool isColored = false;
    
    private float scaledMass;
    private float bounceForce;
    private List <Collision> currentCollisions = new List <Collision> ();
    private Rigidbody rb;

    public void OnCollisionEnter(Collision collision)
    {
        currentCollisions.Add (collision);
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Collider col = GetComponent<Collider>();
        if (!isColored) {
            if (collision.gameObject.tag == "GellBlob") 
            {
                rb.isKinematic = false;
                string gellType = collision.gameObject.GetComponent<BlobCollision>().gellType;
                Destroy(collision.gameObject);
                if (gellType == "BlueGell") 
                {
                    meshRenderer.material = BlueGellMat;
                    // make the object super bouncy
                    col.material.bounciness = 1;
                    currentColor = "blue";

                } else if (gellType == "OrangeGell") 
                {
                    meshRenderer.material = OrangeGellMat;
                    // make the object super slidy
                    col.material.dynamicFriction = 0;
                    currentColor = "orange";
                }
                isColored = true;
            }
        } 
        // if the cude is colored and has collided with an object
        if (meshRenderer.material == BlueGellMat) 
        {
            print("inblue");
            // apply the bounceForce in dir of normal to all colliding objects
            foreach (Collision coll in currentCollisions) 
            {
                foreach (var item in coll.contacts)
                {
                    rb.AddForce(item.normal * bounceForce, ForceMode.Impulse);
                    print(item.normal * bounceForce);
                }
            }
        } 
     }

    // Start is called before the first frame update
    void Start()
    {
        // adjust scale to random if needed
        if (randomizeSize) 
        {
            transform.localScale = new Vector3(Random.value * maxRandomSize, Random.value * maxRandomSize, Random.value * maxRandomSize);
        }

        // adjust the objects mass based on its size
        scaledMass = (transform.localScale.x * transform.localScale.y * transform.localScale.z) / 10;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.mass = scaledMass;
        float bounceForce = player.GetComponent<PlayerMovmentPhysicsBased>().jumpForce * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
