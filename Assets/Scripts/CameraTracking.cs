using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    private Transform TrackedTransform;
    public bool canMove = true;

    private float smoothTime = 0f;
    private float movementTime = 0f;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        SetTrackedObject("PlayerSprite");
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            if (TrackedTransform) {
                if (smoothTime > 0f) {
                    // Debug.Log()
                    this.transform.position = Vector3.SmoothDamp(
                        this.transform.position,
                        TrackedTransform.position + new Vector3(0, 1, -10),
                        ref velocity, 
                        smoothTime
                    );
                    movementTime += Time.deltaTime;

                    if (movementTime >= smoothTime) {
                        smoothTime = 0f;
                    }
                } else {
                    this.transform.position = TrackedTransform.position + new Vector3(0, 1, -10);
                }                
            }
        }
    }

    public void SetTrackedObject(string ObjectName) 
    {
        TrackedTransform = GameObject.Find(ObjectName).transform;
    }

    public void StopTracking()
    {
        canMove = false;
    }

    // public void StartTracking()
    // {
    //     canMove = true;
    //     MoveToTracked(1f);
    // }

    public void StartTracking(float time)
    {
        canMove = true;
        MoveToTracked(time);
    }

    public void MoveToTracked(float time)
    {
        Debug.Log("MOving to position tracked with " + time);
        if (TrackedTransform != null && this.transform.position != TrackedTransform.position) {
            smoothTime = time;
        }
    }
}
