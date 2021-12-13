using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : MonoBehaviour
{

    public GameObject levelPlane;
    // playerGun needed to destroy gell correectly
    // simply destroying the gell object would potentially still leave it in the 
    // list of patches that are interacting with player, even if its destroyed.
    public GameObject playerGun;
    public float waitingTime = 5.0f;
    public float AISpeed = 2f;
    private Bounds navBound;
    private bool lookingForTarget = true;
    private Vector3 currentTargetPosition;
    private float timer = 0.0f;

    Vector3 getRandomPoint() 
    {
        // return a random Vector3 that fits within navBound
        // this creates the random behaviour of the AI agent 
        return new Vector3(
            Random.Range(navBound.min.x, navBound.max.x),
            Random.Range(navBound.min.y, navBound.max.y),
            Random.Range(navBound.min.z, navBound.max.z)
        );
    }

    IEnumerator waitAtPoint(float time) 
    {
        // wait for time seconds 
        yield return new WaitForSeconds(time);
        print("End wait");
        currentTargetPosition = getRandomPoint();
        lookingForTarget = true;
    }

    public void OnTriggerStay(Collider collider)
     {  // this allows for the AI to introduce difficulty to the game
        // by making the agent destroy any gell patches it touches, therefore
        // adding the possability that the player may need to take more steps to
        // complete the level.
        if (collider.gameObject.tag == "OrangeGell" || collider.gameObject.tag == "BlueGell") 
        {
            playerGun.GetComponent<CreateGell>().destroyGellPatch(collider.gameObject);
        }
     }

     public void OnCollisionEnter(Collision other) {
         if (other.gameObject.tag == "Player") 
        {   // this sets the idea that AI's are not invincible, by allowing them to be effected by the player.
            // this creates a counter action for the player and therefor (if we abstract the level a little bit),
            // addin an additional arch the player can make towards the state where there are no AI's, thus increasing
            // the probability that the player will reach the goal (by potentially reducing the number of steps the 
            // player will need to make)
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.freezeRotation = false;
            rb.AddForce(Vector3.up * (other.gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude / 4), ForceMode.Impulse);
        }
     }

    void Start()
    {
        // can add multiple objects to this, and UtilFunctions.getBoundingBox() will generate
        // a bounding box that encompases all of these objects positions
        GameObject[] boundGameObjList = new GameObject[1];
        // we only want the AI to roam around the <levelPlane> object
        boundGameObjList[0] = levelPlane;
        navBound = UtilFunctions.getBoundingBox(boundGameObjList);
        // generate random target to start moving towards   
        currentTargetPosition = getRandomPoint();     
    }


    void Update()
    {
        /*
        At the moment, since this AI is very basic, this doesnt effect the player too much,
        allowing for the player to learn that this gameobject is an AI agent.
        having this excessivly simple AI agent encounter early, allows for possitive 
        feedback to the player that demonstrates this is what an AI is. 
        later on we will introduce harder AI models that build on this one, but for now
        the player can use this level's experiene to better counter some harder AI models. 

        We can create this expected graph of play:

        (Init) node, player can: (Init) -> move towards AI -> (Destroy AI)
                                 (Init) -> move towards Goal -> (Step)
        (Step) node, player can: (Step) -> RND AI hoovers gell needed to complete level -> (Step)
                                 (Step) -> Player places gell and reaches goal (Goal)

        if N = theoretical steps it takes to somplete the level:
        We can see from above, that simply injecting a random chance that the AI will hover the
        gell, this can increase N. However, this is assuming that this specific gell patch would 
        allow the player to complete the level, this is an assumption that we abstract away but 
        is also a factor in N. Things that affect N: player placing gell patches, AI randomness.
        */

        // use objects y value as we do not want AI rotating along X & Z to lookat target if its too close to target
        Vector3 lookAtPos = new Vector3(currentTargetPosition.x, transform.position.y, currentTargetPosition.z);
        transform.LookAt(lookAtPos);

        // simple random AI loop
        if (lookingForTarget) 
        {   // move to a chosen random position
            transform.localPosition = Vector3.MoveTowards (transform.localPosition, currentTargetPosition, AISpeed * Time.deltaTime * 1.2f);
        } 
        // if AI has 'reached' this position (~3f distance)
        if (Vector3.Distance(currentTargetPosition, transform.position) <= 3f) {
            timer += Time.deltaTime;
            if(timer > waitingTime) {
                // wait for <waitingTime> seconds, set new random position, go to this pos in next update loop
                timer = 0f;
                currentTargetPosition = getRandomPoint();
                lookingForTarget = true;
          }
        }
    }
}
