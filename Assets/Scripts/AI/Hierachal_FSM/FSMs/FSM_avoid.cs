using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_avoid : MonoBehaviour
{
    // defencive actions the AI can take

    // vars set by parent (Hierachal FSM)
    // playerGun needed to destroy gell correectly
    // simply destroying the gell object would potentially still leave it in the 
    // list of patches that are interacting with player, even if its destroyed.
    private GameObject playerGun; 
    private state currentState;
    private float playerDetectDistance;
    private float gellDetectDistance;
    private float AISpeed;
    private float AISpeed_max;
    private Vector3 targetPosition;
    private Bounds navBound;
    private float initDistancePlayerFinish; 

    //states
    state dashFromPlayer;
    state moveFromPlayer;


    private void dashFromPlayer_update() 
    {
        // AI does a small 'dash' to avoid the player if it gets too close
        setTargetPosition();
        Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(lookAtPos);
        transform.localPosition = Vector3.MoveTowards (transform.localPosition, targetPosition, (AISpeed * 4) * Time.deltaTime * 1.2f);
    } 

    private void moveFromPlayer_update() 
    {
        setTargetPosition();
        // move from the player at normal move speed (scaled by players distance from finish)        
        // lookat and move AI to target Position
        Vector3 lookAtPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        transform.LookAt(lookAtPos);
        transform.localPosition = Vector3.MoveTowards (transform.localPosition, targetPosition, AISpeed * Time.deltaTime * 1.2f);
    }

    private void setTargetPosition()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - UtilFunctions.getPlayerposition());
        targetPosition = transform.position + transform.forward * playerDetectDistance;
        targetPosition = UtilFunctions.constrictPointToNavBound(navBound, targetPosition);
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

    void OnEnable()
    {
        // set public vars based on vars in parent component
        playerGun = GetComponent<Hierachal_FSM_Control>().playerGun; 
        // half the player detection dist for avoiding, stops state ocillation
        playerDetectDistance = GetComponent<Hierachal_FSM_Control>().playerDetectDistance / 2;
        gellDetectDistance = GetComponent<Hierachal_FSM_Control>().gellDetectDistance;
        AISpeed = GetComponent<Hierachal_FSM_Control>().AISpeed;
        AISpeed_max = GetComponent<Hierachal_FSM_Control>().AISpeed_max;
        navBound = GetComponent<Hierachal_FSM_Control>().navBound;
        initDistancePlayerFinish = UtilFunctions.getPlayerDistanceToFinish(); 
    }

    void Start()
    {
        dashFromPlayer = new state("dashFromPlayer", false, dashFromPlayer_update);
        moveFromPlayer = new state("moveFromPlayer", false, moveFromPlayer_update);
        
        currentState = moveFromPlayer;
    }


    void Update()
    {
        // in update we denote archs to other states through code.
        if (UtilFunctions.isPlayerInRange(transform.position, playerDetectDistance / 2)) 
        {   // priority 1: dash if close
            currentState = dashFromPlayer;
        } else {
            // priority 2: Just move normaly
            currentState = moveFromPlayer;
        }

        // execute the update function of the state we are currently in
        currentState.runStateUpdate();
    }
}
