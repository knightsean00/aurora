using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void ChangeScenery(string name) {
        SceneManager.LoadScene(name);
    }
}

