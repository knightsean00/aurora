using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Raycast raycaster;

    private Rigidbody2D player;
    // private SpriteRenderer renderer;

    private Collider2D groundCollider;

    // private Vector2 velocity
    private float maxSpeed = 12.5f;
    private float jumpVelocity = 15f;

    private float groundAcceleration;
    private float groundDeceleration;
    private float airAcceleration;
    private float airDeceleration;
    private float gravityAcceleration = 90f;
    private bool canMove = true;

    //sounds
    public AudioSource steps;
    public AudioSource echo;

    private float moveInput = 0f;

    private bool isJumping = false;
    private bool jumpToggle = true;
    private float maxGravityDelay = .2f; //This is how maximum length of time gravity will be delayed when jumping
    private float gravityDelayTimer = 0f;

    private int groundMask = 1 << 3;

    private float directionalEcholocationSpan = 30f;

    //Animator Controller
    public Animator animator;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        // renderer = GetComponent<SpriteRenderer>();
        groundCollider = this.transform.Find("GroundCollider").GetComponent<Collider2D>();

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
            animator.SetBool("isJumping", true);
        }
        if (Input.GetButtonUp("Jump")) {
            isJumping = false;
            jumpToggle = true;
        }

        if (Input.GetMouseButtonDown(0)) {
            this.GetComponent<RaycastUI>().RenderCircleCrosshair(raycaster.MaxDistance);
        }
        if (Input.GetMouseButtonUp(0)) {
            raycaster.RunRaycast();
            echo.Play();
            this.GetComponent<RaycastUI>().StopRender();
        }

        if (Input.GetMouseButton(1)) {
            Vector3 mousePosition = this.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)));
            float center = Mathf.Atan2(mousePosition.y, mousePosition.x);
            if (this.transform.localScale.x < 0) {
                center = Mathf.Atan2(mousePosition.y, -mousePosition.x);
            }
            this.GetComponent<RaycastUI>().RenderDirectionalCrosshair(raycaster.MaxDistance, center, Mathf.Deg2Rad * directionalEcholocationSpan);
        }
        if (Input.GetMouseButtonUp(1)) {
            Vector3 mousePosition = this.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)));
            float center = Mathf.Atan2(mousePosition.y, mousePosition.x);
            if (this.transform.localScale.x < 0) {
                center = Mathf.Atan2(mousePosition.y, -mousePosition.x);
            }
            raycaster.RunRaycast(center, Mathf.Deg2Rad * directionalEcholocationSpan);
            echo.Play();
            this.GetComponent<RaycastUI>().StopRender();
        }

        //Animator Controller
        //animator.SetFloat("xDirection", moveInput);
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isJumping", isJumping);
        animator.SetFloat("xDirection", moveInput);
        animator.SetBool("isGrounded", isGrounded());

        if(!isGrounded()){
            steps.enabled = false;
        }

        if (moveInput > 0){
            gameObject.transform.localScale = new Vector3(1,1,1);
            if(isGrounded()){
                steps.enabled = true;
            }
        }
        else {
            if (moveInput < 0){
                gameObject.transform.localScale = new Vector3(-1,1,1);
                if(isGrounded()){
                steps.enabled = true;
                }
            }
            else{
                steps.enabled = false;
            }
        }
        

    }

    void FixedUpdate() 
    {
        if (canMove) {
            Vector2 tempVelocity = player.velocity;

            if (isGrounded()) {
                if (moveInput != 0) {
                    tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, groundAcceleration * Time.fixedDeltaTime);
                } else {
                    tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, groundDeceleration * Time.fixedDeltaTime);
                }

                if (jumpToggle && isJumping) {
                    jumpToggle = false;
                    tempVelocity.y = jumpVelocity;
                    gravityDelayTimer = 0f;
                }
            } else {
                if (moveInput != 0) {
                    tempVelocity.x = Mathf.MoveTowards(tempVelocity.x, maxSpeed * moveInput, airAcceleration * Time.fixedDeltaTime);
                } else {
                    tempVelocity.x = Mathf.MoveTowards(tempVelocity.x , 0, airDeceleration * Time.fixedDeltaTime);
                }

                if (isJumping && gravityDelayTimer < maxGravityDelay && player.velocity.y > 0) {
                    gravityDelayTimer += Time.fixedDeltaTime;
                    // tempVelocity.y = Mathf.MoveTowards(tempVelocity.y, -50, 1f * Time.fixedDeltaTime);
                } else {
                    gravityDelayTimer = maxGravityDelay; //If they let go, then gravity will start again.
                    tempVelocity.y = Mathf.MoveTowards(tempVelocity.y, -50, gravityAcceleration * Time.fixedDeltaTime);
                }
            }

            player.velocity = tempVelocity;
        } else {
            player.velocity = Vector2.zero;
        }
    }

    public bool isGrounded() {
        if (groundCollider.IsTouchingLayers(groundMask)) {
            return true;
        }
        return false;
    }

    public void StopMovement() {
        canMove = false;
    }

    public void StartMovement() {
        canMove = true;
    }
}
