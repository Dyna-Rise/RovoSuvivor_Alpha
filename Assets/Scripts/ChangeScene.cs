using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    public void Load()
    {
        GameManager.playerHP = 10;
        GameManager.shotRemainingNum = 20;
        SceneManager.LoadScene(sceneName);
    }
}
