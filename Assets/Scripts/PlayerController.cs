using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove;
    public Raycast raycaster;

    private Rigidbody2D player;
    private SpriteRenderer renderer;

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

    // private float currentRotation = 0f;

    private int layerMask = 1 << 3;


    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();

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
        if (isGrounded()) {
            if (moveInput != 0) {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, groundAcceleration * Time.fixedDeltaTime);
            } else {
                tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, groundDeceleration * Time.fixedDeltaTime);
            }

            if (isJumping) {
                tempVelocity.y = jumpVelocity;
                gravityDelayTimer = 0f;
            } else {
                // tempVelocity.y = 0;
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
        
        // Debug.Log(tempVelocity);
        player.velocity = tempVelocity;
    }

    bool isGrounded() {
        RaycastHit2D leftRaycast = Physics2D.Raycast(
            new Vector2(player.transform.position.x - (renderer.size.x / 2), player.transform.position.y - (renderer.size.y / 2)),
            Vector2.down,
            5,
            layerMask);
        RaycastHit2D rightRaycast = Physics2D.Raycast(
            new Vector2(player.transform.position.x + (renderer.size.x / 2), player.transform.position.y - (renderer.size.y / 2)),
            Vector2.down,
            5,
            layerMask);

        if (leftRaycast.collider && leftRaycast.distance <= 4 && rightRaycast.collider && rightRaycast.distance <= 4) {
            Vector2 normalDirection = (rightRaycast.normal + leftRaycast.normal) / 2;
            // Vector2 rotationDirection = Vector2.Perpendicular(normalDirection);
            float rotation = (180 / Mathf.PI * Mathf.Atan2(normalDirection.y, normalDirection.x)) - 90;
            player.transform.eulerAngles = new Vector3(0, 0, rotation);

            // Vector2 normalDirection = Vector2.Perpendicular(new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation))) * -1;
            leftRaycast = Physics2D.Raycast(
                new Vector2(player.transform.position.x - (renderer.size.x / 2), player.transform.position.y - (renderer.size.y / 2)),
                normalDirection * -1,
                5,
                layerMask);
            rightRaycast = Physics2D.Raycast(
                new Vector2(player.transform.position.x + (renderer.size.x / 2), player.transform.position.y - (renderer.size.y / 2)),
                normalDirection * -1,
                5,
                layerMask);

            Debug.Log(leftRaycast.distance);
            Debug.Log(rightRaycast.distance);
            if (leftRaycast.distance <= .02 || rightRaycast.distance <= .02) {
                return true;
            }
        } else {
            player.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        
        return false;
    }
}
