using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Detonate : MonoBehaviour
{
    public float ExpireTime = 10f;
    public bool MasterCopy = true;

    public float FadeDelay = 2f;
    public float FadeSpeed = 10f;
    public Transform playerPos;

    private float lastFlash = 0;

    // Start is called before the first frame update
    void Start()
    {
        var mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat("_ScanTime", Time.time);
        lastFlash = Time.time + FadeDelay;
        Update();
    }

    void Update()
    {
        var mat = GetComponent<MeshRenderer>().material;
        mat.SetVector("_Position", playerPos.position);
        mat.SetFloat("_Opacity", 1 / (1 + Mathf.Exp(FadeSpeed * (Time.time - lastFlash))));
        if (!MasterCopy && Time.time - lastFlash >= ExpireTime) Destroy(gameObject);
    }
}

