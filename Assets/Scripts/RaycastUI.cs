using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RenderMode {
    None,
    CircleCrosshair,
    DirectionalCrosshair
}

public class RaycastUI : MonoBehaviour
{
    private RenderMode mode = RenderMode.None;
    private float distance = 0;

    private LineRenderer crosshair;

    void Awake()
    {
        crosshair = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == RenderMode.None) {
            crosshair.enabled = false;
        } else if (mode == RenderMode.CircleCrosshair) {
            crosshair.enabled = true;
            crosshair.positionCount = 361;
            Vector3[] positions = new Vector3[361];
            for (int i = 0; i <= 360; i++) {
                positions[i] = this.transform.position + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), 0) * distance;
            }
            crosshair.SetPositions(positions);
            crosshair.startColor = new Color(1, 1, 1, .25f);
            crosshair.endColor = new Color(1, 1, 1, .25f);
        }
    }

    public void RenderCircleCrosshair(float distance) {
        mode = RenderMode.CircleCrosshair;
        this.distance = distance;
    }

    public void StopRender() {
        mode = RenderMode.None;
    }

    private Vector2 Rotate(Vector2 Original, float Rotation) {
        float rad = Rotation * Mathf.Deg2Rad;
        return new Vector2(Original.x * Mathf.Cos(rad) - Original.y * Mathf.Sin(rad), 
                            Original.x * Mathf.Sin(rad) + Original.y * Mathf.Cos(rad));
    }
}
