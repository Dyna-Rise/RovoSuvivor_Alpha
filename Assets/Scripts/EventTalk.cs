using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventTalk : MonoBehaviour
{
    public TalkData talks;
    public TextMeshProUGUI talkText;
    public string sceneName;

    void Start()
    {
        StartCoroutine(TalkEventStart());
    }

    IEnumerator TalkEventStart()
    {
        for(int i = 0; i < talks.talkDatas.Length; i++)
        {
            talkText.text = talks.talkDatas[i].talk;
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(sceneName);
    }
}
