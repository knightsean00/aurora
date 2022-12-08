using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CollisionSceneChanger : MonoBehaviour
{
    public string SceneName = "";
    

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            SceneManager.LoadScene(SceneName);
        }
    }
}
