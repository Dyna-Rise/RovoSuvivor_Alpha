using UnityEngine;

[System.Serializable]
public class Talk
{
    [TextArea(3, 10)]//最小3行、最大10行でstringを表示
    public string talk;
}

[CreateAssetMenu(fileName = "TalkData", menuName = "TalkData")]
public class TalkData : ScriptableObject
{
    public Talk[] talkDatas;
}