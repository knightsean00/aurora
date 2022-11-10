using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    private Transform TrackedTransform;

    private bool movingToPosition = false;
    private Vector3 targetPosition;
    private float maxDistance = 0f;
    private float movementTime = 0f;

    void Awake()
    {
        SetTrackedObject("PlayerSprite");
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToPosition) {
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                targetPosition + new Vector3(0, 1, -10),
                maxDistance * Time.deltaTime
            );
            movementTime -= Time.deltaTime;

            if (movementTime <= 0) {
                movingToPosition = false;
                targetPosition = Vector3.zero;
                movementTime = 0f;
            }
        } else if (TrackedTransform) {
            this.transform.position = TrackedTransform.position + new Vector3(0, 1, -10);       
        }
    }

    public void SetTrackedObject(string ObjectName) 
    {
        TrackedTransform = GameObject.Find(ObjectName).transform;
    }

    public void StopTracking() 
    {
        TrackedTransform = null;
    }


    public void MoveToPosition(Vector3 position, float time)
    {
        maxDistance = Vector3.Distance(position, this.transform.position) / time;
        movingToPosition = true;
        targetPosition = position;
        movementTime = time;
    }
}
