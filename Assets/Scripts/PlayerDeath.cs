using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Save currentSave;

    void Start()
    {
        currentSave = new Save(this.transform.position, new Inventory());
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Death") {
            Die();
        }
    }

    public void Die() {
        GameObject.Find("Main Camera").SendMessage("StopTracking");
        this.gameObject.GetComponent<PlayerController>().StopMovement();
        this.transform.position = currentSave.position;
        GameObject.Find("Main Camera").SendMessage("StartTracking", 3f);
        Invoke("Respawn", 5f);
    }

    public void Respawn() {
        this.gameObject.GetComponent<PlayerController>().StartMovement();
    }
}

public class Save {
    public Vector3 position;
    Inventory inventory;

    public Save(Vector3 position, Inventory inventory) {
        this.position = position;
        this.inventory = inventory.Copy();
    }
}
