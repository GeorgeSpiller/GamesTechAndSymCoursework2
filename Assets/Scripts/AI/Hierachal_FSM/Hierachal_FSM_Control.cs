using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hierachal_FSM_Control : MonoBehaviour
{
    public float causeAndEffectDelay = 0.5f; 

    // vars to set on all the child FMS's
    public GameObject playerGun; 
    public state currentState;
    public float playerDetectDistance = 10f;
    public float gellDetectDistance = 20f;
    public float AISpeed = 5f;
    public float AISpeed_max = 10f;
    public GameObject levelPlane;
    public float initDistancePlayerFinish;
    public Bounds navBound;

    private float timer = 0.0f;

    // FSM's
    private FSM_avoid avoidActions;
    private FSM_guard guardActions;
    private FSM_idle idleActions;
    private FSM_intercept interceptActions;

    private void dissableAllFSMs() 
    {
        // dissable all FSM's to start with
        avoidActions.enabled = false;
        guardActions.enabled = false;
        idleActions.enabled = false;
        interceptActions.enabled = false;
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
        // all the FSM's this hierachal FSM has access to
        avoidActions = GetComponent<FSM_avoid>();
        guardActions = GetComponent<FSM_guard>();
        idleActions = GetComponent<FSM_idle>();
        interceptActions = GetComponent<FSM_intercept>();

        dissableAllFSMs();

        // can add multiple objects to this, and UtilFunctions.getBoundingBox() will generate
        // a bounding box that encompases all of these objects positions
        GameObject[] boundGameObjList = new GameObject[1];
        // we only want the AI to roam around the <levelPlane> object
        boundGameObjList[0] = levelPlane;
        navBound = UtilFunctions.getBoundingBox(boundGameObjList);
    }

    void Update()
    {
        // used to introduce some 'lag' into the AI to reduce the likelyhood of state ocillation
        timer += Time.deltaTime;
        if(timer > causeAndEffectDelay) {
            float playerFinishDist = UtilFunctions.getPlayerDistanceToFinish();
            float playerDist = Vector3.Distance(transform.position, UtilFunctions.getPlayerposition());
            bool isPlayerCloserToFinish = (playerFinishDist < Vector3.Distance(transform.position, UtilFunctions.getFinishPosition()));
            // wait for delay before executing any logic
            // depending on conditions, dissable/ebable a specific FSM
            if (UtilFunctions.isPlayerInRange(transform.position, playerDetectDistance / 2)) 
            {   // priority 1: avoid player
                if (!avoidActions.enabled) {
                    dissableAllFSMs();
                    Debug.Log("Changed to FSM: avoid");
                    }
                avoidActions.enabled = true;
            } else if(UtilFunctions.getNearestGellPatch(transform.position, gellDetectDistance) != Vector3.zero || isPlayerCloserToFinish) {
                // priority 2: intercept
                if (!interceptActions.enabled) {
                    dissableAllFSMs();
                    Debug.Log("Changed to FSM: intercept");
                    }
                interceptActions.enabled = true;
            // } else if(UtilFunctions.isPlayerInRange(transform.position, playerDetectDistance * 2) && (playerDist > playerDetectDistance)) {
            //     // priority 3: guard
            //     if (!guardActions.enabled) {
            //         dissableAllFSMs();
            //         Debug.Log("Changed to FSM: guard");
            //         }
            // guardActions.enabled = true;
            } else {
                // idle
                if (!idleActions.enabled) {
                    dissableAllFSMs();
                    Debug.Log("Changed to FSM: idle");
                    }
                idleActions.enabled = true;
            }
            timer = 0f;
        }
    }
}
