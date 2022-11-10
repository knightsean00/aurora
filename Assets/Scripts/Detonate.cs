using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Detonate : MonoBehaviour
{
    public float ExpireTime = 10f;

    public float FadeDelay = 2f;
    public float FadeSpeed = 10f;
    public Transform playerPos;

    private float lastFlash = 0;

    // Start is called before the first frame update
    void Start()
    {
        lastFlash = Time.time + FadeDelay;
    }

    // Start is called before the first frame update
    void Update()
    {
        var mat = GetComponent<MeshRenderer>().material;
        mat.SetVector("_Position", playerPos.position);
        mat.SetFloat("_Opacity", 1 / (1 + Mathf.Exp(FadeSpeed * (Time.time - lastFlash))));
        if (Time.time - lastFlash >= ExpireTime) Destroy(gameObject);
    }
}

