using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int Size { get { return inventory.items.Count; } }
    public float Width = 2f;
    public float TurnSpeed = 0.3f;
    private Inventory inventory = new Inventory();

    public Inventory GetInventory() {
        return inventory.Copy();
    }

    public void SetInventory(Inventory newInventory) {
        inventory = newInventory.Copy();
    }
    
    public void ResetInventory(Inventory newInventory) {
        foreach(var item in inventory.GetItems()) {
            if (!newInventory.HasItem(item)) {
                item.Release();
            }
        }
        inventory = newInventory.Copy();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "RuneDoor") {
            Debug.Log(collision.gameObject.GetComponent<RuneDoor>().RequiredRunes);
            if (inventory.HasItems(collision.gameObject.GetComponent<RuneDoor>().RequiredRunes)) {
                HideObject(collision.gameObject);
            } else {
                Debug.Log(inventory);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        var obj = collider.gameObject;
        var collectible = obj.GetComponent<Collectible>();
        if (collectible != null) {
            foreach (var item in inventory.items) {
                item.Perturb(Collectible.GrabState.Shifting);
            }
            inventory.AddItem(collectible);
            collectible.Grab(Size - 1, this);
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



public class Inventory
{
    public HashSet<Collectible> items = new HashSet<Collectible>();

    public Inventory() { }
    
    public Inventory(HashSet<Collectible> items) {
        this.items = new HashSet<Collectible>(items);
    }

    public void AddItem(Collectible item) {
        items.Add(item);
    }

    public void AddItems(IEnumerable<Collectible> items) {
        foreach (Collectible item in items) {
            this.AddItem(item);
        }
    }

    public void RemoveItem(Collectible item) {
        items.Remove(item);
    }

    public void RemoveItems(IEnumerable<Collectible> items) {
        foreach (Collectible item in items) {
            this.RemoveItem(item);
        }
    }

    public bool HasItem(Collectible item) {
        return items.Contains(item);
    }

    public bool HasItems(IEnumerable<Collectible> items) {
        bool contains = true;
        foreach (Collectible item in items) {
            contains = contains && this.HasItem(item);
        }
        return contains;
    }

    public HashSet<Collectible> GetItems() {
        return new HashSet<Collectible>(items);
    }

    public Inventory Copy() {
        return new Inventory(items);
    }

    public override string ToString() {
        string output = "{";
        foreach(Collectible item in items) {
            output += item + ", ";
        }
        return output + "}";
    }
}
