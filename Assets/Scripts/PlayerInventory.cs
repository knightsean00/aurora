using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    public int Size { get { return inventory.Count; } }
    public List<Collectible> inventory = new List<Collectible>();

    public List<Collectible> GetInventory() {
        return new List<Collectible>(inventory);
    }

    public void ResetInventory(List<Collectible> newInventory) {
        foreach(var item in inventory) {
            if (!newInventory.Contains(item)) {
                item.Release();
            }
        }
        inventory = new List<Collectible>(newInventory);
    }

    public Transform GetTrans(int ix) {
        if (ix == 0) return transform;
        if (ix < 0) return null;
        return inventory[ix - 1].transform;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "RuneDoor") {
            var door = collision.gameObject.GetComponent<RuneDoor>();
            var pos = door.transform.localPosition;
            if (door.RequiredRunes.All(rune => inventory.Contains(rune))) {
                foreach (var (rune, ix) in inventory.Select((val, index) => (val, index))) {
                    var targetTime = Time.time + door.Timing * (Size - ix - 1);
                    rune.move = new MoveStrategy.TimedDelay(targetTime, rune.move, ix == 0
                        ? () => new MoveStrategy.ReleaseOpen(targetTime, rune.transform.localPosition, pos, door)
                        : () => new MoveStrategy.Release(targetTime, rune.transform.localPosition, pos));
                }
            } else {
                door.ChangeColor(new Color(0.6320754f, 0.1156817f, 0.1710251f));
                Debug.Log(inventory);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "RuneDoor") {
            collision.gameObject.GetComponent<RuneDoor>().DefaultColor();
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        var obj = collider.gameObject;
        var collectible = obj.GetComponent<Collectible>();
        if (collectible != null) {
            collectible.Grab(this);
            inventory.Add(collectible);
            Debug.Log(inventory);
        }
    }

    public void HideObject(GameObject gameObject) {
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ShowObject(GameObject gameObject) {
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
