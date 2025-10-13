using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    playing,
    pause,
    option,
    gameover,
    gameclear,
}

public enum Phase
{
    gate,
    enemy,
    boss
}

public class GameManager : MonoBehaviour
{
    public static GameState gameState;
    public static int playerHP = 10;
    public static int shotRemainingNum = 20;

    public static int numOfGate;

    public static Phase missionPhase;
    bool onBossStage; //ボスステージフラグ
    bool onNormalStage; //通常ステージフラグ

    public List<GameObject> enemyList = new List<GameObject>();

    void Start()
    {
        gameState = GameState.playing;
        //最初にゲートの数を把握
        numOfGate = GameObject.FindGameObjectsWithTag("Gate").Length;

        //シーン情報の取得
        Scene currentScene = SceneManager.GetActiveScene();
        //シーン名の取得
        string sceneName = currentScene.name;

        switch (sceneName)
        {
            case "BossStage":
                missionPhase = Phase.boss;
                onBossStage = true;
                SoundManager.instance.PlayBgm(BGMType.Boss);
                break;
            case "MainStage":
                missionPhase = Phase.gate;
                onNormalStage = true;
                SoundManager.instance.PlayBgm(BGMType.Stage);
                break;
            case "Opening":
                SoundManager.instance.PlayBgm(BGMType.Opening);
                break;
            case "Ending":
                SoundManager.instance.PlayBgm(BGMType.Ending);
                break;
            case "Title":
                SoundManager.instance.PlayBgm(BGMType.Title);
                break;
        }
    }

    private void Update()
    {
        if(numOfGate <= 0 && !onBossStage && onNormalStage)
        {
            missionPhase = Phase.enemy;
            if(enemyList.Count <= 0)
            {
                //すべての敵を殲滅したらボスステージへ
                StartCoroutine(StartBossStage());
            }
        }

        if(gameState == GameState.gameclear)
        {

            //ボスを殲滅したらエンディングへ
            StartCoroutine(StartEnding());
        }
    }

    IEnumerator StartBossStage()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("BossStage");
    }

    IEnumerator StartEnding()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Ending");
    }
}
