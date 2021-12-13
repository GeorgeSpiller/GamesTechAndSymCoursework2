using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_guard : MonoBehaviour
{
    public float guardAreaSize = 5f;
    public Bounds guardBounds;
    Vector3 boudnds_axis_posX;
    Vector3 boudnds_axis_negX;
    Vector3 boudnds_axis_posZ;
    Vector3 boudnds_axis_negZ;

    // vars set by parent (Hierachal FSM)
    // playerGun needed to destroy gell correectly
    // simply destroying the gell object would potentially still leave it in the 
    // list of patches that are interacting with player, even if its destroyed.
    public GameObject playerGun; 
    public state currentState;
    public float playerDetectDistance;
    public float gellDetectDistance;
    public float AISpeed;
    public float AISpeed_max;
    private Vector3 targetPosition;
    private Bounds navBound;
    private float initDistancePlayerFinish; 
    private float currGuardIndex = 0;
    private float timer = 0.0f;
    private bool guardState = false;

    // FSM states
    state idleGuard;
    state guardPointNearPlayer;

    // state Update Functions
    private void idleGuard_update() 
    {
        // walk from two edges of guardBounds along axis X
        // walk back to center
        // walk from two edges of guardBounds along axis Z
        //walk back to center
        
        switch(currGuardIndex) 
        {
            case 0:
                targetPosition = boudnds_axis_negX;
                moveToTargetPosition();
                break;
            case 1:
                targetPosition = boudnds_axis_posX;
                moveToTargetPosition();
                break;
            case 2:
                targetPosition = guardBounds.center;
                moveToTargetPosition();
                break;
            case 3:
                targetPosition = boudnds_axis_negZ;
                moveToTargetPosition();
                break;
            case 4:
                targetPosition = boudnds_axis_posZ;
                moveToTargetPosition();
                break;
            case 5:
                targetPosition = guardBounds.center;
                moveToTargetPosition();
                currGuardIndex = 0;
                break;
            default:
                break;
        }

    }

    private void guardPointNearPlayer_update() 
    {
        // walk to edge of bound where player was last 'seen'
        targetPosition = UtilFunctions.getPlayerposition();
        targetPosition = UtilFunctions.constrictPointToNavBound(guardBounds, targetPosition);
        moveToTargetPosition();
    }

    private void moveToTargetPosition()
    {
        // move to targetLocation if its within navBound
        if (navBound.Contains(targetPosition)) 
        {
            Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.LookAt(lookAtPos);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
        } else {
            // print("position not in nav: " + targetPosition);
            // if the gell patch is not within the navBounds of the AI, do not move towards it
            //targetPosition = Vector3.zero;
            Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.LookAt(lookAtPos);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
        }
        // if AI has completed a move to a gell patch, reset targetPosition
        if (Vector3.Distance(transform.position, targetPosition) < 1f) {
            targetPosition = Vector3.zero;
            currGuardIndex += 1;
        }
    }

    private void regenerateGuardingBounds() 
    {
        // used for guarding a specific area 
        guardBounds = new Bounds(transform.position, new Vector3(guardAreaSize, guardAreaSize, guardAreaSize));
        // mid =  (v1 + v2) / 2;
        // get all four corners of the guard bounds (in 2D with y just being AI's y)
        Vector3 boudnds_corner_XZ = new Vector3(guardBounds.max.x, transform.position.y, guardBounds.max.z);
        Vector3 boudnds_corner_mXmZ = new Vector3(guardBounds.min.x, transform.position.y, guardBounds.min.z);
        Vector3 boudnds_corner_mXZ = new Vector3(guardBounds.min.x, transform.position.y, guardBounds.max.z);
        Vector3 boudnds_corner_XmZ = new Vector3(guardBounds.max.x, transform.position.y, guardBounds.min.z);

        // convert these corners to edges
        boudnds_axis_posX = (boudnds_corner_XZ + boudnds_corner_XmZ) / 2;
        boudnds_axis_negX = (boudnds_corner_mXZ + boudnds_corner_mXmZ) / 2;
        boudnds_axis_posZ = (boudnds_corner_mXZ + boudnds_corner_XZ) / 2;
        boudnds_axis_negZ = (boudnds_corner_mXmZ + boudnds_corner_XmZ) / 2;

        /* Visual representation of what we are calculating above
        (mXZ) - - - - <posZ>- - - - - (XZ)
          |                          |
          <negX>                     <posX>
          |                          |
        (mXmZ) - - - - <negZ>- - - - (XmZ)

         /\
       Z |  
         # ---->
            X
        */
    }

    void OnEnable()
    {
        // set public vars based on vars in parent component
        playerGun = GetComponent<Hierachal_FSM_Control>().playerGun; 
        playerDetectDistance = GetComponent<Hierachal_FSM_Control>().playerDetectDistance;
        gellDetectDistance = GetComponent<Hierachal_FSM_Control>().gellDetectDistance;
        AISpeed = GetComponent<Hierachal_FSM_Control>().AISpeed;
        AISpeed_max = GetComponent<Hierachal_FSM_Control>().AISpeed_max;
        navBound = GetComponent<Hierachal_FSM_Control>().navBound;
        initDistancePlayerFinish = UtilFunctions.getPlayerDistanceToFinish(); 

        regenerateGuardingBounds();
    }

    void Start()
    {
        idleGuard = new state("idleGuard", false, idleGuard_update);
        guardPointNearPlayer = new state("guardPointNearPlayer", false, guardPointNearPlayer_update);

        currentState = idleGuard;   
    }



    void Update()
    {

        // after 5s, the AI has a 50% chance to iether pertroll a region, or stay at the  point thats close to the player
        timer += Time.deltaTime;
        if (timer > 5) {
            if(Random.value < .5) 
            {   
                guardState  = guardState  ? false : true;
            }
            timer = 0.0f;
        }
        currentState = guardState  ? idleGuard : guardPointNearPlayer;

        // execute the update function of the state we are currently in
        currentState.runStateUpdate();
    }
}
