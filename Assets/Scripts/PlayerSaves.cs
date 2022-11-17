using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    private Save currentSave;
    private float resetTime = .75f;

    void Awake()
    {
        currentSave = new Save(
            this.transform.position, 
            this.GetComponent<PlayerInventory>().GetInventory()
        );

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            if (enemy != null) {
                enemy.GetComponent<EnemyBehavior>().CreateSave();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        HandleCollision(collision.gameObject);
    }

    void HandleCollision(GameObject collisionObject) {
        if (collisionObject.tag == "Respawn") {
            currentSave.position = collisionObject.transform.position;
            currentSave.inventory = this.GetComponent<PlayerInventory>().GetInventory();
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
                enemy.GetComponent<EnemyBehavior>().CreateSave();
            }
        } else if (collisionObject.tag == "Death") {
            Die();
        } else if (collisionObject.tag == "HiddenDeath") {
            Die();
        } else if (collisionObject.tag == "Enemy") {
            Die();
        }
    }

    public void Die() {
        this.gameObject.GetComponent<PlayerController>().StopMovement();
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<PlayerInventory>().ResetInventory(currentSave.inventory);
        this.transform.position = currentSave.position;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            enemy.GetComponent<EnemyBehavior>().LoadSave();
        }
        
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
