using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    private Save currentSave;
    private float resetTime = .75f;

    void Awake()
    {
        currentSave = new Save(this.transform.position, this.GetComponent<PlayerInventory>().GetInventory());
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Death") {
            Die();
        }
    }

    public void Die() {
        this.gameObject.GetComponent<PlayerController>().StopMovement();
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<PlayerInventory>().ResetInventory(currentSave.inventory);
        Debug.Log("Resetting Inventory to " + currentSave.inventory);
        this.transform.position = currentSave.position;
        

        GameObject.Find("Main Camera").GetComponent<CameraTracking>().MoveToPosition(currentSave.position, resetTime);
        Invoke("Respawn", resetTime + .1f);
    }

    public void Respawn() {
        this.gameObject.GetComponent<PlayerController>().StartMovement();
        this.GetComponent<SpriteRenderer>().enabled = true;
    }
}

public class Save {
    public Vector3 position;
    public Inventory inventory;

    public Save(Vector3 position, Inventory inventory) {
        this.position = position;
        this.inventory = inventory.Copy();
    }
}
