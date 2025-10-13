using UnityEngine;

//BGMタイプ
public enum BGMType
{
    None,
    Title,
    Opening,
    Stage,
    Boss,
    Ending
}

//SEタイプ
public enum SEType
{
    Shot,
    Damage,
    Jump,
    Sword,
    Walk,
    Create,
    Explosion,
    Barrier,
    Tackle,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    BGMType playingBGM;
    AudioSource audioSource;

    public AudioClip titleBGM;
    public AudioClip openingBGM;
    public AudioClip stageBGM;
    public AudioClip bossBGM;
    public AudioClip endingBGM;

    //現シーンでインスタンスを作成
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンが切り替わっても破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

    }

    //BGM再生
    public void PlayBgm(BGMType type)
    {
        if (type != playingBGM)
        {
            playingBGM = type;

            switch (type)
            {
                case BGMType.Title:
                    audioSource.clip = titleBGM;
                    audioSource.Play();
                    break;
                case BGMType.Opening:
                    audioSource.clip = openingBGM;
                    audioSource.Play();
                    break;
                case BGMType.Stage:
                    audioSource.clip = stageBGM;
                    audioSource.Play();
                    break;
                case BGMType.Boss:
                    audioSource.clip = bossBGM;
                    audioSource.Play();
                    break;
                case BGMType.Ending:
                    audioSource.clip = endingBGM;
                    audioSource.Play();
                    break;
            }
        }
    }


    //停止メソッド
    public void StopBgm()
    {
        GetComponent<AudioSource>().Stop();
        playingBGM = BGMType.None;
    }
}
