using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Color temp = this.GetComponent<SpriteRenderer>().color;
        this.GetComponent<SpriteRenderer>().color = new Color(temp.r, temp.g, temp.b, 1000f);
        Debug.Log(this.GetComponent<SpriteRenderer>().color.a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
