using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Inventory inventory = new Inventory();

    public Inventory GetInventory() {
        return inventory.Copy();
    }

    public void SetInventory(Inventory newInventory) {
        inventory = newInventory.Copy();
    }
    
    public void ResetInventory(Inventory newInventory) {
        foreach(string itemName in inventory.GetItems()) {
            if (!newInventory.HasItem(itemName)) {
                ShowObject(GameObject.Find(itemName));
            }
        }
        inventory = newInventory.Copy();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Collectible") {
            inventory.AddItem(collision.gameObject.name);
            HideObject(collision.gameObject);
            Debug.Log(inventory);
        } else if (collision.gameObject.tag == "RuneDoor") {
            Debug.Log(collision.gameObject.GetComponent<RuneDoor>().RequiredRunes);
            if (inventory.HasItems(collision.gameObject.GetComponent<RuneDoor>().RequiredRunes)) {
                HideObject(collision.gameObject);
            } else {
                Debug.Log(inventory);
            }
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
    private HashSet<string> items = new HashSet<string>();

    public Inventory() { }
    
    public Inventory(HashSet<string> items) {
        this.items = new HashSet<string>(items);
    }

    public void AddItem(string itemName) {
        items.Add(itemName);
    }

    public void AddItems(IEnumerable<string> itemNames) {
        foreach (string itemName in itemNames) {
            this.AddItem(itemName);
        }
    }

    public void RemoveItem(string itemName) {
        items.Remove(itemName);
    }

    public void RemoveItems(IEnumerable<string> itemNames) {
        foreach (string itemName in itemNames) {
            this.RemoveItem(itemName);
        }
    }

    public bool HasItem(string itemName) {
        return items.Contains(itemName);
    }

    public bool HasItems(IEnumerable<string> itemNames) {
        bool contains = true;
        foreach (string itemName in itemNames) {
            contains = contains && this.HasItem(itemName);
        }
        return contains;
    }

    public HashSet<string> GetItems() {
        return new HashSet<string>(items);
    }

    public Inventory Copy() {
        return new Inventory(items);
    }

    public override string ToString() {
        string output = "{";
        foreach(string itemName in items) {
            output += itemName + ", ";
        }
        return output + "}";
    }
}
