using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    private Transform TrackedTransform;
    public bool canMove = true;


    void Awake()
    {
        SetTrackedObject("PlayerSprite");
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            if (TrackedTransform) {
                this.transform.position = TrackedTransform.position + new Vector3(0, 1, -10);
            }
        }
    }

    public void SetTrackedObject(string ObjectName) 
    {
        TrackedTransform = GameObject.Find(ObjectName).transform;
    }
}
