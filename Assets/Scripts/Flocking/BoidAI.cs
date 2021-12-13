using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidAI : MonoBehaviour
{

    [Header("Boid General Settings (Hover for info)")]
    [Tooltip("Defines the bounds that the boid can fly within. Bound extent = boundScalar * viewRange.")]
    public float boundScalar = 2;

    [Tooltip("The view range of the Boid.")]
    public float viewRange = 5f;

    [Tooltip("Maximum velocity allowed for the boid.")]
    public float maxSpeed = 2f;

    [Header("Boid Scalar Settings (Hover for info)")]
    [Tooltip("Separation: How much the Boid should separate from other friendly boids.")]
    [Range(0.0f, 1f)]
    public float separationWeight = 0.8f;

    [Range(0.0f, 1f)]
    [Tooltip("Cohesion: How much the Boid should move towards the average position of its flock.")]
    public float cohesionWeight = 0.5f;

    [Range(0.0f, 1f)]
    [Tooltip("Alignment: How much the Boid should fly in the same direction of its flock.")]
    public float alignmentWeight = 0.7f;

    [Range(0.0f, 1f)]
    [Tooltip("Avoidance: How much the Boid should avoid any obsticals (any object thats has a different tag).")]
    public float avoidanceWeight = 2.9f;
    
    [Range(0.0f, 1f)]
    [Tooltip("Attractor: How much the Boid should move towards objects that attract it (any object that has the 'attractor_<color> tag').")]
    public float attractorWeight = 5f;


    private Bounds bound;
    private Vector3 mVelocity = new Vector3();
    private List<GameObject> BoidNeighbours;
    private Vector3 cohesionVector;
    private Vector3 separationVector;
    private Vector3 alignmentVector;
    private Vector3 avoidanceVector;
    private Vector3 attractorVector;

    private Vector3 allFlockingBehaviours() 
    {
        /*
        This is the main function that calculates all the different vectors used by the boids.
        Thes vectors include:
            -   separationVector: Points away from nearby flock members
            -   alignmentVector: Points towards the facing direction of nearby flock members
            -   avoidanceVector: Points away from obsticals
            -   attractorVector: Points towards attracting objects
        Thes vectors all get normalised, scaled with their weights and compinded into one movment vector.
        */

        string attractorTag = "BoidAttractor_" + gameObject.tag.Split(char.Parse("_"))[1];
        Collider[] visableEnteties = Physics.OverlapSphere(transform.position, viewRange);
        List<GameObject> theFlock = new List<GameObject>();
        List<GameObject> attractors = new List<GameObject>();
        List<Vector3> obsticals = new List<Vector3>();

        // Loop through all objects within the boids view range, and catogrise the objects as iether
        // other friendly boids, attractors or obsticalls
        foreach(Collider hitCol in visableEnteties) 
        {
            string gmObjTag = hitCol.gameObject.tag;
            if (gmObjTag == gameObject.tag) 
            {
                // If the object is a member of the same flock, add to flock list
                theFlock.Add(hitCol.gameObject);
            } else if (gmObjTag == attractorTag || gmObjTag == "GellSplatter") 
            {
                // if the object is an attractor (gellsplatter)
                attractors.Add(hitCol.gameObject);
            } else {
                if (gmObjTag != "Enviroment" ) 
                {   
                    // All other objects appart from the level plane are obsticals.
                    // Make boids ignore floor as obstical, so they can destroy gell
                    obsticals.Add(hitCol.ClosestPointOnBounds(transform.position));
                }
            }
        }


        Vector3 flockingVector = new Vector3();
        // reduce the weights over time, reduces ocillation
        cohesionVector /= 2;
        separationVector /= 2;
        alignmentVector /= 2;
        avoidanceVector /= 2;

        // these are used to calculate averages of objects for boids (cohesion), attractors (attraction) and obsticals (avoidance)
        int frendlyCount = 0;
        int kindaFriendlyCount = 0;
        int notSoFriendlyCount = 0;
        
        // calculate vectors that are associated with the flock (cohesion, separation, alignment)
        for(int flockIndex = 0; flockIndex < theFlock.Count; flockIndex++)
        {
            // the distance between the boid and the nearly flock boid
            float distance = (transform.position - theFlock[ flockIndex ].transform.position).sqrMagnitude;
            if (theFlock[flockIndex].gameObject != gameObject) 
            {
                if( distance > 0 )
                {
                    // if the nearby flock boid is not the current boid, calculate the flock-orientated vectors
                    cohesionVector += theFlock[flockIndex].transform.position;
                    separationVector += theFlock[flockIndex].transform.position - transform.position;
                    alignmentVector += theFlock[flockIndex].transform.forward;
                    frendlyCount++; // used to average the cohesion
                }
            }
        }

        // loop through attractors, calculate attractionVector
        for(int attraIndex = 0; attraIndex < attractors.Count; attraIndex++)
        {
            attractorVector += attractors[attraIndex].transform.position;
            kindaFriendlyCount++; // used to average attractor
        }

        // calculate vectors that are not associated with the flock (avoidance)
        for (int obsIndex = 0; obsIndex < obsticals.Count; obsIndex++) 
        {
            float distance = (transform.position - obsticals[obsIndex]).sqrMagnitude;
            if (distance > 0) 
                {
                    avoidanceVector += obsticals[obsIndex] - transform.position;
                    notSoFriendlyCount++; // used to average avoidance
                }
        }

        // set count variables (used in averages) to 1 if they are 0 (avoids divide by 0)
        frendlyCount = frendlyCount == 0 ? 1 : frendlyCount;
        kindaFriendlyCount = kindaFriendlyCount == 0 ? 1 : kindaFriendlyCount;
        notSoFriendlyCount = notSoFriendlyCount == 0 ? 1 : notSoFriendlyCount;

        // Take the average of the respective vectors
        separationVector /= frendlyCount;
        alignmentVector /= frendlyCount;
        cohesionVector /= frendlyCount;
        attractorVector /= kindaFriendlyCount;
        // avoidanceVector /= notSoFriendlyCount;

        // invert seperation and avoidance (so they move away rather than towards)
        separationVector *= -1;
        avoidanceVector *= -1;

        // convert into a localised direction vector (to take into account the average pos of a cluster of boids)
        cohesionVector = (cohesionVector - transform.position);
        attractorVector = (attractorVector - transform.position);

        // print(
        //     "separation: " + ( separationVector.normalized * separationWeight ) + 
        //     ", cohesion: " + ( cohesionVector.normalized * cohesionWeight ) + 
        //     ", alignment: " + ( alignmentVector.normalized * alignmentWeight ) +
        //     ", avoidance:" + ( avoidanceVector.normalized * avoidanceWeight ) );

        // normalise, apply weights and combind all vectors into one Uber direction (velocity) vector.
        flockingVector = ( ( separationVector.normalized * separationWeight ) +
                                    ( cohesionVector.normalized * cohesionWeight ) +
                                    ( alignmentVector.normalized * alignmentWeight ) +
                                    ( avoidanceVector.normalized * avoidanceWeight ) + 
                                    ( attractorVector.normalized * attractorWeight ) );
        return flockingVector;
    }

    void Start()
    {
        // randomise the position slightly, but keep the boids within bounds scaled with their view range
        transform.position += Random.insideUnitSphere * (viewRange * boundScalar);
        bound = new Bounds(transform.position, new Vector3(viewRange * boundScalar, viewRange * boundScalar, viewRange * boundScalar));
    }

    void Update()
    {
        // Calculate the boids movment vector
        mVelocity += allFlockingBehaviours();
        // clamp speed of boid
        mVelocity = Vector3.ClampMagnitude(mVelocity, maxSpeed);
        // rotate boid towards velocity direction (for alignment calculations)
        transform.LookAt(transform.position + mVelocity);

        // keep the boid within its bounds
        if (!bound.Contains(transform.position)) 
        {
            mVelocity += (bound.center - transform.position).normalized;
        }

        // finally, apply velocity to boid.
        transform.position += mVelocity * Time.deltaTime;

    }
}
