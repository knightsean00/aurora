using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneDoor : MonoBehaviour
{
    public List<Collectible> RequiredRunes = new List<Collectible>();
    public float Timing = 1f;
    private float animationTime = .25f;
    private float timer = .25f;
    private Color targetColor;
    private Color oldColor = new Color(0.5288537f, 0.5324851f, 0.6886792f, 1f);
    private static Color defaultColor = new Color(0.5288537f, 0.5324851f, 0.6886792f, 1f);

    //Audio
    public AudioSource collect;

    void Awake() {
        this.GetComponent<SpriteRenderer>().color = defaultColor;
    }

    void Update() {
        if (targetColor != null && timer < animationTime) {
            timer += Time.deltaTime;
            this.GetComponent<SpriteRenderer>().color = Color.Lerp(oldColor, targetColor, timer / animationTime);
        } else if (timer >= animationTime) {
            oldColor = targetColor;
        }
    }

    public void DefaultColor() {
        this.ChangeColor(defaultColor);
    }

    public void ChangeColor(Color color) {
        this.targetColor = color;
        timer = 0;
    }
    public void Open() {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        foreach (var collectible in RequiredRunes) {
            collect.Play();
            collectible.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
