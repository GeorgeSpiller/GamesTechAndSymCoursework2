using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovmentPhysicsBased : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float airMoveSpeed = 2f;
    public float orangeGellMoveSpeed = 18f;
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 1;
    public float groundDrag = 6f;
    public float airDrag = 2f;
    public bool isOnSpeedGell = false;
    public GameObject LevelRespawn;
    public bool hasAppliedJump = false;

    private float playerHeight = 2f;
    private float airMultiplier = 0.4f;
    private float movementMultiplier = 10f;
    private bool isGrounded;
    private float horizontalMovement;
    private float verticalMovement;
    private Vector3 moveDirection;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // calculate if the player is grounded, so that different drag forces are applied based on if the 
        // player is in the air or not. (Also used in jumping mechanics).
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2f + 0.1f);

        // changes the movment direction venctor based on input
        MyInput();
        // changes the drag forces that are applied to the players rigid body 
        ControlDrag();

        // handel jump
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            // impuse as its a sudden force using its mass
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlDrag()
    {
        // set different drag values depending on if the player is in the sair or on the ground.
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        // move the player in Fixed as its physics based movment
        MovePlayer();
    }

    void MovePlayer()    
    {
        if (isGrounded)
        {
            // apply different force multiplyers depending on if the player is on a speed gell patch or not
            if (isOnSpeedGell) {
                // orange gell movment
                rb.AddForce(moveDirection.normalized * orangeGellMoveSpeed * movementMultiplier, ForceMode.Acceleration);
                FindObjectOfType<AudioManager>().orangeFadeIn("orangegell");
            } else {
                // normal movment
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                FindObjectOfType<AudioManager>().orangeFadeOut("orangegell");
            }
        }
        else if (!isGrounded)
        {
            // if the player is in the air, reduce movment as there is much less traction
            rb.AddForce(moveDirection.normalized * airMoveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }
}
