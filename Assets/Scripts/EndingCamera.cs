using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCamera : MonoBehaviour
{
    public GameObject vc1;
    public GameObject vc2;
    public GameObject vc3;
    public GameObject vc4;

    public float waitCamera1;
    public float waitCamera2;
    public float waitCamera3;

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
        yield return new WaitForSeconds(waitCamera1);
        vc1.SetActive(false);

        yield return new WaitForSeconds(waitCamera2);
        vc2.SetActive(false);
        
        yield return new WaitForSeconds(waitCamera3);

        talkCanvas.SetActive(true);

        for (int i = 0; i < talks.talkDatas.Length; i++)
        {
            talkText.text = talks.talkDatas[i].talk;
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
        talkCanvas.SetActive(false);
        vc3.SetActive(false);

        yield return new WaitForSeconds(10);

        SceneManager.LoadScene(sceneName);

    }
}
