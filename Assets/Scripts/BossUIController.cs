using UnityEngine;
using UnityEngine.UI;

public class BossUIController : MonoBehaviour
{
    public BossController bossCnt; //ボスのスクリプト
    public Slider bossSlider;

    int currentBossHP;

    void Start()
    {
        bossSlider.maxValue = bossCnt.bossHP;        
        bossSlider.value = bossCnt.bossHP;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBossHP != bossCnt.bossHP)
        {
            currentBossHP = bossCnt.bossHP;
            bossSlider.value = currentBossHP;
        }
    }
}
