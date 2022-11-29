using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Collectible : MonoBehaviour
{
    public Vector2 defaultPos;
    public Vector2 lastPos;
    public float lastTheta;
    public int ix;
    public PlayerInventory inventory = null;
    public static float transformTime = 1f;
    public static float transformPow = 2f;

    public float grabTime = -1e20f;
    public enum GrabState {
        Grabbing,
        Grabbed,
        Shifting,
        Releasing,
        Released
    }

    public GrabState state = GrabState.Released;

    void Start() {
        defaultPos = transform.localPosition;
    }
    public void Perturb(GrabState grab) {
        grabTime = Time.time;
        lastPos = transform.localPosition;
        state = grab;
    }
    public void Grab(int ix, PlayerInventory inventory) {
        if (state.ReleaseState()) {
            GetComponent<Collider2D>().enabled = false;
            this.ix = ix;
            this.inventory = inventory;
            Perturb(GrabState.Grabbing);
        }
    }
    public void Release() {
        if (!state.ReleaseState()) {
            state = GrabState.Releasing;
            this.inventory = null;
            Perturb(GrabState.Releasing);
        }
    }
    public void Update() {
        GrabState nextState = state.AfterState();
        float dt = (Time.time - grabTime) / transformTime;
        float expDt = (Mathf.Pow(transformPow, dt) - 1) / (transformPow - 1);
        float theta;
        Vector3 hoverPos = Vector3.zero;
        if (inventory != null) {
            float baseTheta = 2 * Mathf.PI * ix / inventory.Size;
            if (state == GrabState.Shifting) {
                theta = Mathf.Lerp(lastTheta, baseTheta, dt);
            } else {
                theta = lastTheta = baseTheta;
            }
            theta += Time.time / inventory.TurnSpeed;
            hoverPos = inventory.gameObject.transform.localPosition + new Vector3(inventory.Width * Mathf.Cos(theta), inventory.Width * Mathf.Sin(theta), 0);
        }
        switch (state) {
            case GrabState.Grabbing:
                this.transform.localPosition = Vector3.Lerp(lastPos, hoverPos, expDt); break;
            case GrabState.Grabbed:
            case GrabState.Shifting:
                this.transform.localPosition = hoverPos; break;
            case GrabState.Releasing:
                this.transform.localPosition = Vector3.Lerp(lastPos, defaultPos, expDt); break;
            case GrabState.Released:
                GetComponent<Collider2D>().enabled = true;
                this.transform.localPosition = defaultPos; break;
        }
        if (dt >= 1) {
            state = nextState;
        }
    }
}

public static class Extensions {
    public static bool ReleaseState(this Collectible.GrabState grab) {
        return grab == Collectible.GrabState.Releasing || grab == Collectible.GrabState.Released;
    }
    public static Collectible.GrabState AfterState(this Collectible.GrabState grab) {
        if (grab == Collectible.GrabState.Grabbing || grab == Collectible.GrabState.Shifting) return Collectible.GrabState.Grabbed;
        if (grab == Collectible.GrabState.Releasing) return Collectible.GrabState.Released;
        return grab;
    }
}
