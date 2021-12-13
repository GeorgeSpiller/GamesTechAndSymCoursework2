using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_AI : MonoBehaviour
{
    // playerGun needed to destroy gell correectly
    // simply destroying the gell object would potentially still leave it in the 
    // list of patches that are interacting with player, even if its destroyed.
    public GameObject playerGun; 
    public state currentState;
    public float playerDetectDistance = 5f;
    public float gellDetectDistance = 20f;
    public float AISpeed = 2f;
    public float AISpeed_max = 10f;
    public GameObject levelPlane;

    // states
    private state idle;
    private state moveToGell;
    private state runFromPlayer;

    
    // Sences
    private Vector3 targetPosition;
    private Bounds navBound;
    private float initDistancePlayerFinish; 


    // state Update Functions
    // do not pass any parameters into them as we want the only info the AI to 
    // know is info a 'player' woud know, ie. info about itself (position ect.)

    private void idle_update() {
    
    }

    private void moveToGell_update() {
        // if gell is within navounds
        if (navBound.Contains(targetPosition)) 
        {
            Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.LookAt(lookAtPos);
            transform.localPosition = Vector3.MoveTowards (transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
        } else {
            // if the gell patch is not within the navBounds of the AI, do not move towards it
            targetPosition = Vector3.zero;
        }
        // if AI has completed a move to a gell patch, reset targetPosition
        if (Vector3.Distance(transform.position, targetPosition) < 1f) {
            targetPosition = Vector3.zero;
        }
    }

    private void runFromPlayer_update() {
        transform.rotation = Quaternion.LookRotation(transform.position - UtilFunctions.getPlayerposition());
        targetPosition = transform.position + transform.forward * playerDetectDistance;
        targetPosition = UtilFunctions.constrictPointToNavBound(navBound, targetPosition);
        
        // lookat and move AI to target Position
        Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(lookAtPos);
        transform.localPosition = Vector3.MoveTowards (transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
    }

    private void scaleSpeed() 
    {   // scale the movment speed of the AI based on the players distance to the finish.
        // this introduces a level of difficulty, as the closer the player gets to the goal,
        // the faster the AI is. The faster the AI is, the increased propability it has to
        // remove a players gell. Cap this speed at a max

        // linearly scale speed to be between [0, getPlayerDistanceToFinish], based on inversePlayerDistToGoal
        float inversePlayerDistToGoal = initDistancePlayerFinish - (UtilFunctions.getPlayerDistanceToFinish());
        float[] r = new float[] {1, initDistancePlayerFinish}; // the measurment (player distance) scale
        float[] t = new float[] {1, AISpeed_max}; // the target (AI Speed) scale
        AISpeed = (((inversePlayerDistToGoal - r[0]) / (r[1] - r[0])) * (t[1] - t[0])) + t[0];
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
        initDistancePlayerFinish = UtilFunctions.getPlayerDistanceToFinish();

        idle = new state("idle", false, idle_update);
        moveToGell = new state("moveToGell", false, moveToGell_update);
        runFromPlayer = new state("runFromPlayer", false, runFromPlayer_update);

        currentState = idle;

        // can add multiple objects to this, and UtilFunctions.getBoundingBox() will generate
        // a bounding box that encompases all of these objects positions
        GameObject[] boundGameObjList = new GameObject[1];
        // we only want the AI to roam around the <levelPlane> object
        boundGameObjList[0] = levelPlane;
        navBound = UtilFunctions.getBoundingBox(boundGameObjList);
    }


    void Update() 
    {
        // in update we denote archs to other states through code.
        if (UtilFunctions.isPlayerInRange(transform.position, playerDetectDistance)) 
        {   // priority 1: run from player
            currentState = runFromPlayer;
        } else {   
            // sorry for the long variable names please dont mark me down :C
            bool gellInRange = (UtilFunctions.getNearestGellPatch(transform.position, gellDetectDistance) != Vector3.zero);
            if (gellInRange || targetPosition != Vector3.zero) 
            {   // priority 2: hoover up gell
                // get closest gell patch
                targetPosition = UtilFunctions.getNearestGellPatch(transform.position, gellDetectDistance);
                currentState = moveToGell;
            } else {
                // if nothing else, sit idle
                currentState = idle;
            }
        }

        // execute the update function of the state we are currently in
        currentState.runStateUpdate();

        // scale AI speed based on players distance to the finish
        scaleSpeed();
    }
}
