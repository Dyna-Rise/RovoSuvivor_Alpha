using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI lifeValue;
    public Slider lifeSlider;

    public TextMeshProUGUI rifleValue;
    public Slider rifleSlider;

    DashController dash;
    public Slider dashSlider;

    public TalkData missions;
    public GameObject missionPanel;
    public TextMeshProUGUI missionText;

    int currentPlayerHP;
    int currentShotRemainingNum;
    float currentDashStamina;

    public GameObject lifePanel;
    public GameObject riflePanel;
    public GameObject dashPanel;
    public GameObject controllerPanel;
    public GameObject cursorPanel;

    public GameObject gameOverPanel;

    bool isGameOverPanel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dash = GameObject.FindGameObjectWithTag("Player").GetComponent<DashController>();

        currentPlayerHP = GameManager.playerHP;
        int val = currentPlayerHP * 100;
        lifeValue.text = val.ToString();
        lifeSlider.value = currentPlayerHP;

        currentShotRemainingNum = GameManager.shotRemainingNum;
        rifleValue.text = currentShotRemainingNum.ToString();
        rifleSlider.value = currentShotRemainingNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameState.gameover)
        {
            if (!isGameOverPanel)
            {
                isGameOverPanel = true;

                Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
                Cursor.visible =    true; //カーソルを表示

                lifePanel.SetActive(false);
                riflePanel.SetActive(false);
                dashPanel.SetActive(false);
                controllerPanel.SetActive(false);
                cursorPanel.SetActive(false);
                missionPanel.SetActive(false);

                gameOverPanel.SetActive(true);
            }
            return;
        }

        if (currentPlayerHP != GameManager.playerHP)
        {
            currentPlayerHP = GameManager.playerHP;
            int val = currentPlayerHP * 100;
            lifeValue.text = val.ToString();
            lifeSlider.value = currentPlayerHP;
        }

        if (currentShotRemainingNum != GameManager.shotRemainingNum)
        {
            currentShotRemainingNum = GameManager.shotRemainingNum;
            rifleValue.text = currentShotRemainingNum.ToString();
            rifleSlider.value = currentShotRemainingNum;
        }

        switch (GameManager.missionPhase)
        {
            case Phase.gate:
                missionText.text = missions.talkDatas[0].talk;
                break;
            case Phase.enemy:
                missionText.text = missions.talkDatas[1].talk;
                break;
            case Phase.boss:
                missionText.text = missions.talkDatas[2].talk;
                break;
        }

        //ダッシュを実装できた後に機能
        if (dash != null)
        {
            if (currentDashStamina != dash.currentDashStamina)
            {
                currentDashStamina = dash.currentDashStamina;
                dashSlider.value = currentDashStamina;
            }
        }

    }
}
