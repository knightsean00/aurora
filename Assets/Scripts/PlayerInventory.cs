using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Inventory
{
    private HashSet<string> items = new HashSet<string>();

    public Inventory() { }
    
    public Inventory(HashSet<string> items) 
    {
        this.items = new HashSet<string>(items);
    }

    public void AddItem(string itemName) 
    {
        items.Add(itemName);
    }

    public void AddItems(IEnumerable<string> itemNames) 
    {
        foreach (string itemName in itemNames) {
            this.AddItem(itemName);
        }
    }

    public void RemoveItem(string itemName)
    {
        items.Remove(itemName);
    }

    public void RemoveItems(IEnumerable<string> itemNames)
    {
        foreach (string itemName in itemNames) {
            this.RemoveItem(itemName);
        }
    }

    public bool HasItem(string itemName) 
    {
        return items.Contains(itemName);
    }

    public bool HasItems(IEnumerable<string> itemNames)
    {
        bool contains = true;
        foreach (string itemName in itemNames) {
            contains = contains && this.HasItem(itemName);
        }
        return contains;
    }

    public Inventory Copy() 
    {
        return new Inventory(items);
    }
}
