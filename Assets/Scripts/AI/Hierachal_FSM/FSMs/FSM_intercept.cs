using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_intercept : MonoBehaviour
{

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
    public Bounds navBound;

    private Vector3 targetPosition;
    private float initDistancePlayerFinish; 

    // FSM states
    state dashToHooverGell;
    state moveToHooverGell;
    state blockPlayer;

    // offencive actions the AI can take
    private void dashToHooverGell_update() 
    {
        // AI does a 'dash' towards targetLocation to hoover gell
        // just increase the speed temporarily
        float prev_AISpeed = AISpeed;
        AISpeed = AISpeed * 4;
        moveToHooverGell_update();
        AISpeed = prev_AISpeed;
    }

    private void moveToHooverGell_update() 
    {
        // AI moves normally towards targetLocation to hoover gell
        // set targetLocation to Vector3.zero uppon sucessful hoover
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

    private void blockPlayer_update() 
    {
        // stand between the player and the finish
        // get vector3 thats inbetween player and finish
        Vector3 playerPosition = UtilFunctions.getPlayerposition();
        Vector3 finishPosition = UtilFunctions.getFinishPosition();
        // get midpoint
        targetPosition = (playerPosition + finishPosition) / 2;
        // constrict to nav Bounds
        targetPosition = UtilFunctions.constrictPointToNavBound(navBound, targetPosition);
        if (Vector3.Distance(transform.position, targetPosition) < 1f) {
            targetPosition = Vector3.zero;
        } else {
            // move AI to target position, only if not already there
            Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.LookAt(lookAtPos);
            transform.localPosition = Vector3.MoveTowards (transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
        }
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
    }

    void Start()
    {
        dashToHooverGell = new state("dashToHooverGell", false, dashToHooverGell_update);
        moveToHooverGell = new state("moveToHooverGell", false, moveToHooverGell_update);
        blockPlayer = new state("blockPlayer", false, blockPlayer_update);

        currentState = blockPlayer;
    }

    void Update()
    {
        // if AI is closer to gell than player, hoover it
        Vector3 playerPos = UtilFunctions.getPlayerposition();
        Vector3 gellPos = UtilFunctions.getNearestGellPatch(transform.position, gellDetectDistance);
        float playerDist = Vector3.Distance(gellPos, playerPos);
        float gellDist;
        bool switchToBlock = false;
        if (gellPos != Vector3.zero) 
        {
            gellDist =  Vector3.Distance(gellPos, transform.position);
        } else {
            gellDist = 0;
            switchToBlock = true;
        }

        if (playerDist > gellDist && !switchToBlock) 
        {
            targetPosition = gellPos;
            // if there is not much space in it, dash
            if ((playerDist - gellDist) < 3f) 
            {
                currentState = dashToHooverGell;
            } else {
                currentState = moveToHooverGell;
            }
        } else {
            // player is closer to gell patch, stand between player and finish instead
            currentState = blockPlayer;
        }

        // execute the update function of the state we are currently in
        currentState.runStateUpdate();
    }
}
