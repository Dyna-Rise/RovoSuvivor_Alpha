using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject gate; //Gateオブジェクト
    public GameObject bulletPrefab; //バレットのプレハブ
    public float shootPower = 100f; //ショットパワー
    bool isAttack; //攻撃中フラグ

    public float shotRecoverTime = 0.2f; //回復時間

    private void Update()
    {
        //左クリックかつ、攻撃フラグがOFF
        if (Input.GetMouseButtonDown(0) && !isAttack) Shot();
    }

    //ショットメソッド
    void Shot()
    {
        //攻撃中フラグをON
        isAttack = true;
        //弾を生成
        GameObject obj = Instantiate(
            bulletPrefab,
            gate.transform.position,
            gate.transform.rotation * Quaternion.Euler(90, 0, 0)
            );

        //カメラの方向に弾を飛ばす
        Vector3 v = Camera.main.transform.forward;
        v.y += 0.2f; //照準どおりに飛ぶように調整

        obj.GetComponent<Rigidbody>().AddForce(v * shootPower, ForceMode.Impulse);
        //回復時間タイマーを開始
        StartCoroutine(ShotRecoverCoroutine());
    }

    //回復時間
    IEnumerator ShotRecoverCoroutine()
    {
        //回復時間後に再度打てるようになる
        yield return new WaitForSeconds(shotRecoverTime);
        isAttack = false;
    }
}
