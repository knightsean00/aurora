using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behavior {
    MoveXAndStop,
    MoveXAndContinue,
    Drop,
    DropXAndStop,
    DropXAndContinue
}

public class EnemyBehavior : MonoBehaviour
{
    private bool isChasing = false;
    private bool isEnabled = true;
    private Vector2 target;
    private Rigidbody2D enemy;
    private float gravityScale = 2.5f;

    public float speed;
    public Behavior enemyBehavior;

    private EnemySave save;
    
    void Awake() {
        enemy = GetComponent<Rigidbody2D>();
        if (enemyBehavior == Behavior.Drop || 
                enemyBehavior == Behavior.DropXAndStop ||
                enemyBehavior == Behavior.DropXAndContinue) {
            enemy.gravityScale = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isChasing) {
            if (enemyBehavior == Behavior.MoveXAndStop) {
                // enemy.velocity = Vector2.zero;
                Vector3 tempPosition = this.transform.position;
                tempPosition.x = Mathf.MoveTowards(
                    tempPosition.x,
                    target.x,
                    speed * Time.deltaTime);
                this.transform.position = tempPosition;

                Vector2 currentPosition = tempPosition;
                if (currentPosition == target){
                    isChasing = false;
                }
            } else if (enemyBehavior == Behavior.MoveXAndContinue) {
                Vector2 tempVelocity = enemy.velocity;
                tempVelocity.x = speed;
                enemy.velocity = tempVelocity;
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
        if (collisionObject.tag == "Death") {
            Despawn();
        } else if (collisionObject.tag == "HiddenDeath") {
            Despawn();
        }
    }

    public void Despawn() {
        isEnabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;
        enemy.velocity = Vector2.zero;
        enemy.gravityScale = 0f;
        isChasing = false;
    }

    public void LoadSave() {
        enemy.velocity = Vector2.zero;
        this.speed = save.speed;
        this.isChasing = save.isChasing;
        this.transform.position = save.position;
        this.target = save.target;
        enemy.gravityScale = save.gravityScale;
        this.enemyBehavior = save.enemyBehavior;
        this.isEnabled = save.isEnabled;
        if (save.isEnabled) {
            this.GetComponent<SpriteRenderer>().enabled = true;
            this.GetComponent<Collider2D>().enabled = true;
        } else {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void CreateSave() {
        save = new EnemySave(
            isChasing,
            isEnabled,
            this.transform.position,
            target,
            GetComponent<Rigidbody2D>().gravityScale,
            speed,
            enemyBehavior
        );
    }

    public void StartChasing(Vector2 position) {
        target = position;
        isChasing = true;

        if (enemyBehavior == Behavior.Drop || 
                enemyBehavior == Behavior.DropXAndStop ||
                enemyBehavior == Behavior.DropXAndContinue) {
            enemy.gravityScale = gravityScale;
        }
        if (enemyBehavior == Behavior.DropXAndContinue) {
            enemyBehavior = Behavior.MoveXAndContinue;
        }
        if (enemyBehavior == Behavior.DropXAndStop) {
            enemyBehavior = Behavior.MoveXAndStop;
        }

        if (enemyBehavior == Behavior.MoveXAndContinue) {
            if (this.transform.position.x < target.x) {
                speed = Mathf.Abs(speed);
            } else if (this.transform.position.x > target.x) {
                speed = -Mathf.Abs(speed);
            }
        }
    }
}

public class EnemySave {
    public bool isChasing;
    public bool isEnabled;
    public Vector2 position;
    public Vector2 target;
    public float gravityScale;
    public float speed;
    public Behavior enemyBehavior;

    public EnemySave(bool isChasing, bool isEnabled, Vector2 position, Vector2 target, float gravityScale, float speed, Behavior enemyBehavior) {
        this.isChasing = isChasing;
        this.isEnabled = isEnabled;
        this.position = position;
        this.target = target;
        this.gravityScale = gravityScale;
        this.speed = speed;
        this.enemyBehavior = enemyBehavior;
    }
}