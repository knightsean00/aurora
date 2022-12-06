using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleWisp : MonoBehaviour
{
    public float TimeOffset = 2f;
    public float GlowTime = 1f;
    public float FlashInTime = 0.6f;
    public float GlowMax = 80f;
    public bool Ready = false;
    private Material material;
    public SpriteRenderer overlayMaterial;
    public static Vector3 pos;
    public GameObject player;
    public GameObject reveal;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(15, 0);
        GetComponent<Rigidbody2D>().inertia *= 3f;
        material = GetComponent<SpriteRenderer>().material;
        reveal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var glowTime = clamp((Time.time - TimeOffset) / GlowTime, 0f, 1f);
        var color = material.color;
        color.a = Mathf.Pow(GlowMax, glowTime);
        material.color = color;
        var flashTime = 1f - Mathf.Abs(clamp((Time.time - (TimeOffset + GlowTime)) / FlashInTime, -1f, 1f));
        if (!Ready && TimeOffset + GlowTime < Time.time) {
            Ready = true;
            SetupTitle();
        }
        color = overlayMaterial.color;
        color.a = flashTime;
        overlayMaterial.color = color;
    }
    void SetupTitle() {
        if (pos == new Vector3())
            pos = transform.localPosition;
        player.transform.localPosition = pos;
        reveal.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
    }
    public static float clamp(float val, float min, float max) {
        return min > val ? min : max < val ? max : val;
    }
}
