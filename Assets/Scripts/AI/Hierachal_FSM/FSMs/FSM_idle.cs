using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_idle : MonoBehaviour
{
    public Material mat_main;
    public Material mat_toggle;
    public float textureChangeDuration = 2f;
    public state currentState;
    private bool colorSwitch;
    private float timer = 0.0f;
    private float changeMatTimer = 0.0f;

    private Renderer noseRender;
    private bool idleState = true; 

    // FMS states
    state spinAround;
    state changeMat;

    // state Update Functions
    private void spinAround_update() 
    {
        // simple animation that spins the AI around (to denote its idle)
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }

    private void changeMat_update() 
    {
        // changes the texture of the AI's nose to idicate that it is idle
        // after a delay, toggle texture
        changeMatTimer += Time.deltaTime;
        if(changeMatTimer > textureChangeDuration) {
            colorSwitch = colorSwitch ? false : true;
            changeMatTimer = 0.0f;
        }

        if (colorSwitch) 
        {
            noseRender.material = mat_main;
        } else {
            noseRender.material = mat_toggle;
        }
    }

    void OnDisable() {
        // we need to reset the texture of the nose to its main texture 
        transform.GetChild(0).GetComponent<Renderer>().material = mat_main;
    }

    void Start()
    {
        spinAround = new state("spinAround", false, spinAround_update);
        changeMat = new state("changeMat", false, changeMat_update);

        currentState = spinAround;

        noseRender = transform.GetChild(0).GetComponent<Renderer>();
    }

    void Update()
    {
        // after a period of time, the AI has a 50% chance to iether spin, or flash its nose
        timer += Time.deltaTime;
        if (timer > textureChangeDuration * 2) {
            float rndVal = Random.value;
            if(rndVal < .5) 
            {   
                idleState = idleState ? false : true;
            }
            timer = 0.0f;
        }
        currentState = idleState ? changeMat : spinAround;
            // execute the update function of the state we are currently in
        currentState.runStateUpdate();
    }
}
