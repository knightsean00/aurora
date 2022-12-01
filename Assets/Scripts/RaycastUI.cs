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
    private float center = 0f;
    private float span = 0f;
    private float transitionSpeed = .1f;
    private float timer = 0f;
    private float startAlpha = 0f;
    private float endAlpha = .25f;

    private LineRenderer crosshair;

    void Awake()
    {
        crosshair = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] positions = new Vector3[2];
        if (mode == RenderMode.None) {
            if (timer > 0) {
                timer -= Time.deltaTime;
            } else {
                timer = 0f;
                crosshair.enabled = false;
            }
        } else if (mode == RenderMode.CircleCrosshair) {
            crosshair.enabled = true;
            crosshair.loop = true;
            crosshair.positionCount = 361;
            positions = new Vector3[crosshair.positionCount];
            for (int i = 0; i <= 360; i++) {
                positions[i] = this.transform.position + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), 0) * distance;
            }
        } else if (mode == RenderMode.DirectionalCrosshair) {
            crosshair.enabled = true;
            crosshair.loop = false;
            crosshair.positionCount = 3;
            positions = new Vector3[crosshair.positionCount];

            positions[0] = this.transform.position + new Vector3(Mathf.Cos(center + span), Mathf.Sin(center + span), 0) * distance;
            positions[1] = this.transform.position;
            positions[2] = this.transform.position + new Vector3(Mathf.Cos(center - span), Mathf.Sin(center - span), 0) * distance;
        }

        if (mode != RenderMode.None) {
            crosshair.SetPositions(positions);

            if (timer < transitionSpeed) {
                timer += Time.deltaTime;
            } else {
                timer = transitionSpeed;
            }
        }

        float currentAlpha = (endAlpha * (timer / transitionSpeed)) + startAlpha;
        crosshair.startColor = new Color(1, 1, 1, currentAlpha);
        crosshair.endColor = new Color(1, 1, 1, currentAlpha);
    }

    public void RenderCircleCrosshair(float distance) {
        mode = RenderMode.CircleCrosshair;
        this.distance = distance;
    }

    public void RenderDirectionalCrosshair(float distance, float center, float span) {
        mode = RenderMode.DirectionalCrosshair;
        this.distance = distance;
        this.center = center;
        this.span = span;
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
