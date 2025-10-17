using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject gate; //Gateオブジェクト
    public GameObject bulletPrefab; //バレットのプレハブ
    public float shootPower = 100f; //ショットパワー
    bool isAttack; //攻撃中フラグ
    public float shotRecoverTime = 7.0f; //リロード時間

    AudioSource audioSource;
    [SerializeField] AudioClip se_shot;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.gameState != GameState.playing)
        {
            return;
        }

        //左クリックかつ、攻撃フラグがOFF
        if (Input.GetMouseButtonDown(0) && !isAttack) Shot();
    }

    //ショットメソッド
    void Shot()
    {
        //残弾数が0なら何もしない
        if (GameManager.shotRemainingNum <= 0)
        {
            return;
        }

        GameManager.shotRemainingNum--;
        isAttack = true;
        //ショット音を鳴らす
        audioSource.PlayOneShot(se_shot);

        //弾を生成
        GameObject obj = Instantiate(
            bulletPrefab,
            gate.transform.position,
            gate.transform.rotation * Quaternion.Euler(90, 0, 0)
            );

        //カメラの方向に弾を飛ばす
        Vector3 v = Camera.main.transform.forward;
        //照準どおりに飛ぶように調整
        v.y += 0.2f;

        obj.GetComponent<Rigidbody>().AddForce(v * shootPower, ForceMode.Impulse);
        //リロードを開始
        StartCoroutine(ShotRecoverCoroutine());
        //連射できないように0.2秒後に攻撃フラグをOFF
        Invoke("CanShot", 0.2f);
    }

    //硬直解除
    void CanShot()
    {
        isAttack = false;
    }

    //リロード
    IEnumerator ShotRecoverCoroutine()
    {
        //リロード時間後に弾を補充する
        yield return new WaitForSeconds(shotRecoverTime);
        GameManager.shotRemainingNum++;
    }
}
