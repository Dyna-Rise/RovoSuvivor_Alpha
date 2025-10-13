using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningCamera : MonoBehaviour
{
    public GameObject vc1;
    public GameObject vc2;

    public GameObject talkCanvas;

    public TalkData talks;
    public TextMeshProUGUI talkText;
    public string sceneName;

    void Start()
    {
        StartCoroutine(EventCameraAngle());   
    }

    IEnumerator EventCameraAngle()
    {
        yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < talks.talkDatas.Length; i++)
            {
                talkText.text = talks.talkDatas[i].talk;
                while (!Input.GetKeyDown(KeyCode.Space))
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.1f);
            }

        yield return new WaitForSeconds(1);

        vc1.SetActive(false);
        talkCanvas.SetActive(false);

        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(sceneName);


    }
}
