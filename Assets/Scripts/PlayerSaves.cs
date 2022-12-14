using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    private Save currentSave;
    private float resetTime = .75f;

    //sounds
    public AudioSource respawn;

    void Awake()
    {
        currentSave = new Save(
            this.transform.position, 
            this.GetComponent<PlayerInventory>().GetInventory()
        );
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
                enemy.SendMessage("CreateSave");
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
            enemy.SendMessage("LoadSave");
        }
        
        respawn.Play();
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
    public List<Collectible> inventory;

    public Save(Vector3 position, List<Collectible> inventory) {
        this.position = position;
        this.inventory = new List<Collectible>(inventory);
    }
}
