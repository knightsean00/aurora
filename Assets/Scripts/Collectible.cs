using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Collectible : MonoBehaviour
{
    public Vector2 defaultPos;

    public MoveStrategy move = null;

    void Start() {
        defaultPos = transform.localPosition;
    }
    public void Grab(PlayerInventory inventory) {
        GetComponent<Collider2D>().enabled = false;
        int n = inventory.Size;
        move = new MoveStrategy.Grab(inventory.GetTrans(n), inventory.GetTrans(n - 1), Vector3.zero);
    }
    public void Release() {
        if (move != null) {
            move = new MoveStrategy.Release(Time.time, transform.localPosition, defaultPos);
        }
    }
    public void Update() {
        if (move != null) {
            transform.localPosition = move.tick(this);
        }
    }
}
