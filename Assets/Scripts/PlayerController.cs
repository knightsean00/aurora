using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove;

    private Rigidbody2D player;

    // private Vector2 velocity
    private float maxSpeed = 10f;
    private float jumpVelocity = 10f;

    private float groundAcceleration;
    private float groundDeceleration;
    private float airAcceleration;
    private float airDeceleration;

    private float moveInput = 0f;

    private bool isJumping = false;
    private float maxGravityDelay = .25f; //This is how long gravity will be delayed
    private float gravityDelayTimer = 0f;
    private float gravity = 1f;

    private bool grounded = true;


    void Start()
    {
        player = GetComponent<Rigidbody2D>();

        groundAcceleration = maxSpeed * 10;
        groundDeceleration = groundAcceleration * 2f;

        airAcceleration = maxSpeed * 4;
        airDeceleration = maxSpeed * 2;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            isJumping = true;
        }
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
        }
        // Debug.Log(isJumping);
    }

    void FixedUpdate() 
    {
        Vector2 tempVelocity = player.velocity;

        if (tempVelocity.y == 0f) {
            Debug.Log(isJumping);
            if (moveInput != 0) {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, groundAcceleration * Time.fixedDeltaTime);
            } else {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, groundDeceleration * Time.fixedDeltaTime);
            }

            if (isJumping) {
                grounded = false;
                tempVelocity.y = jumpVelocity;
                player.gravityScale = .5f;
                gravityDelayTimer = 0f;
                // player.gravityScale = Mathf.MoveTowards(player.gravityScale, gravity, )
            }
        } else {
            if (moveInput != 0) {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, airAcceleration * Time.fixedDeltaTime);
            } else {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, airDeceleration * Time.fixedDeltaTime);
            }

            if (isJumping && gravityDelayTimer < maxGravityDelay) {
                gravityDelayTimer += Time.fixedDeltaTime;
                player.gravityScale = Mathf.MoveTowards(player.gravityScale, gravity, .01f);
            } else {
                gravityDelayTimer = maxGravityDelay; //If they let go, then gravity will start again.
                player.gravityScale = Mathf.MoveTowards(player.gravityScale, gravity, .05f);
            }
        }
        
        // Debug.Log(tempVelocity);
        player.velocity = tempVelocity;
    }

    bool isGrounded() {
        return true;
    }
}
