using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove;
    public Raycast raycaster;

    private Rigidbody2D player;

    // private Vector2 velocity
    private float maxSpeed = 10f;
    private float jumpVelocity = 10f;

    private float groundAcceleration;
    private float groundDeceleration;
    private float airAcceleration;
    private float airDeceleration;
    private float gravityAcceleration = 35f;

    private float moveInput = 0f;

    private bool isJumping = false;
    private float maxGravityDelay = .3f; //This is how maximum length of time gravity will be delayed when jumping
    private float gravityDelayTimer = 0f;

    private float coyoteTimeDelay = 1f;
    private float coyoteTimer = 0f;

    private int layerMask = 1 << 3;


    void Start()
    {
        player = GetComponent<Rigidbody2D>();

        groundAcceleration = maxSpeed * 9;
        groundDeceleration = groundAcceleration * 2f;

        airAcceleration = maxSpeed * 4f;
        airDeceleration = maxSpeed * 2f;
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
        if (Input.GetMouseButtonDown(0)) {
            raycaster.RunRaycast();
        }
    }

    void FixedUpdate() 
    {
        Vector2 tempVelocity = player.velocity;
        // Debug.Log(coyoteTimer);

        if (isGrounded()) {
            if (moveInput != 0) {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, groundAcceleration * Time.fixedDeltaTime);
            } else {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, groundDeceleration * Time.fixedDeltaTime);
            }

            if (isJumping) {
                tempVelocity.y = jumpVelocity;
                gravityDelayTimer = 0f;
            }
        } else {
            if (moveInput != 0) {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, airAcceleration * Time.fixedDeltaTime);
            } else {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, airDeceleration * Time.fixedDeltaTime);
            }

            if (isJumping && gravityDelayTimer < maxGravityDelay) {
                gravityDelayTimer += Time.fixedDeltaTime;
                // tempVelocity.y = Mathf.MoveTowards(tempVelocity.y, -50, 1f * Time.fixedDeltaTime);
            } else {
                gravityDelayTimer = maxGravityDelay; //If they let go, then gravity will start again.
                tempVelocity.y = Mathf.MoveTowards(tempVelocity.y, -50, gravityAcceleration * Time.fixedDeltaTime);
            }
        }

        if (!isGrounded() && coyoteTimer < coyoteTimeDelay) {
            coyoteTimer += Time.fixedDeltaTime;
        }
        
        // Debug.Log(tempVelocity);
        player.velocity = tempVelocity;
    }

    bool isGrounded() {
        RaycastHit2D raycastResult = Physics2D.Raycast(player.transform.position, Vector2.down, 5, layerMask);

        if (raycastResult.collider && raycastResult.distance <= .52) {
            coyoteTimer = 0f;
            return true;
        }
        return false;
    }
}
