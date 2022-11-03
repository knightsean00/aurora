using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector2 start;
    public Vector2 end;
    public float movementTime;
    public bool linear;
    public bool repeat = true;

    private Vector2 velocity = new Vector2(0, 0); //only used by smooth damp
    private Vector2 target;

    void Awake() 
    {
        this.transform.position = new Vector3(start.x, start.y, this.transform.position.z);
        target = end;
    }

    void FixedUpdate()
    {
        Vector2 curPosition = this.transform.position;
        Vector2 newPosition;

        if (repeat) {
            if (curPosition == start) {
                target = end;
            } else if (curPosition == end) {
                target = start;
            }
        }

        if (linear) {
            newPosition = Vector2.MoveTowards(curPosition, target, Vector2.Distance(start, end) / movementTime * Time.fixedDeltaTime);
        } else {
            newPosition = Vector2.SmoothDamp(curPosition, target, ref velocity, movementTime, Mathf.Infinity, Time.fixedDeltaTime);
        }

        this.transform.position = new Vector3(newPosition.x, newPosition.y, this.transform.position.z);
    }

    public Vector2 GetVelocity() {
        if (linear) {
            return new Vector2((target.x - start.x) / movementTime, (target.y - start.y) / movementTime);
        } else {
            return velocity;
        }
    }
}
