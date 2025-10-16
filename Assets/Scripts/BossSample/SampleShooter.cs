using System.Collections;
using UnityEngine;

public class SampleShooter : MonoBehaviour
{
    public GameObject gate; //Gateオブジェクト
    public GameObject bulletPrefab; //バレットのプレハブ
    public float shotPower = 100f; //ショットパワー
    bool isAttack; //攻撃中フラグ

    public float shotRecoverTime = 3.0f; //回復時間

    void Update()
    {
        //プレイイング中でなければ
        if (GameManager.gameState != GameState.playing) return;

        //左クリックが押されたら
        if (Input.GetMouseButtonDown(0))
        {
            Shot(); //攻撃メソッド
        }
    }

    //攻撃メソッド
    void Shot()
    {
        if (isAttack) return; //すでに攻撃中なら何もしない

        //残数がなければ何もしない
        if (GameManager.shotRemainingNum <= 0) return;

        isAttack = true; //攻撃中フラグを立てる
        GameManager.shotRemainingNum--; //残数を減らす

        // Gateの回転にX軸90度だけ回転
        Quaternion bulletRotation = gate.transform.rotation * Quaternion.Euler(90, 0, 0);

        //弾の生成
        GameObject obj = Instantiate(
            bulletPrefab,
            gate.transform.position,
            bulletRotation);

        //弾のRigidbodyを取得
        Rigidbody rbody = obj.GetComponent<Rigidbody>();

        // ショットの向きをカメラ方向から少し上向きに調整
        Vector3 shotDirection = Camera.main.transform.forward + (new Vector3(0, 0.2f, 0));

        // 計算した方向にショット
        rbody.AddForce(shotDirection * shotPower, ForceMode.Impulse);

        StartCoroutine(RecoverShot()); //残数の回復コルーチン

        //一定時間待ってから攻撃フラグを解除
        Invoke("AttackOFF", 0.2f);
    }

    //残数の回復コルーチン
    IEnumerator RecoverShot()
    {
        //一定時間待つ
        yield return new WaitForSeconds(shotRecoverTime);
        GameManager.shotRemainingNum++; //残数回復
    }

    //攻撃フラグ解除
    void AttackOFF()
    {
        isAttack = false; //攻撃フラグを解除
    }
}