using System.Collections; // コルーチンのために必要
using UnityEngine;

public class Gate : MonoBehaviour
{
    GameObject player;
    public float detectionRange = 80f;     // プレイヤーを検知する距離

    public int gateHP = 10;
    public GameObject enemyPrefab; //
    public float generateInterval = 20.0f; //生成インターバル

    float timer; //経過時間

    public GameObject body;　//点滅対象
    bool isDamage; //ダメージフラグ

    public GameObject explosionPrefab; //爆発エフェクト

    GameObject gameMgr;

    AudioSource audioSource;

    public AudioClip se_Create;
    public AudioClip se_Explesion;

    private void Start()
    {
        //Player情報を取得
        player = GameObject.FindGameObjectWithTag("Player");
        gameMgr = GameObject.FindGameObjectWithTag("GM");

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.gameState != GameState.playing) return;
        if (player == null) return;
        if (gateHP <= 0) return;


        // ダメージ中の点滅処理
        if (isDamage)
        {
            Blinking();
        }

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // プレイヤーが検知範囲内にいるか
        if (distanceToPlayer <= detectionRange)
        { 
            timer += Time.deltaTime; //時間経過
        }

        //時間がきたらEnemyを生成
        if (timer >= generateInterval)
        {
            audioSource.PlayOneShot(se_Create);

            GameObject obj = Instantiate(
                enemyPrefab,
                transform.position,
                Quaternion.identity
                );

            //リストにEnemy情報を追加
            gameMgr.GetComponent<GameManager>().enemyList.Add(obj);

            timer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーの攻撃に当たったら
        if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("PlayerSword"))
        {
            //ダメージ中なら何もしない
            if (isDamage) return;

            //接近攻撃なら
            if (other.gameObject.CompareTag("PlayerSword"))
            {
                gateHP -= 3; //敵のHPを2つマイナス
            }
            else
            {
                gateHP--; //敵のHPをマイナス
            }

            isDamage = true; //ダメージフラグON

            if(gateHP <= 0)
            {
                GameManager.numOfGate--; //ゲートの数を減らす

                //爆発エフェクトの生成
                Instantiate(
                    explosionPrefab,
                    transform.position,
                    Quaternion.identity
                    );

                audioSource.PlayOneShot(se_Explesion);

                Destroy(gameObject, 0.5f); //ゲートの消滅
            }

            //ダメージリセット
            StartCoroutine(DamageReset());
        }
    }

    //ダメージリセットのコルーチン
    IEnumerator DamageReset()
    {
        yield return new WaitForSeconds(1.0f); // 点滅時間

        isDamage = false; //ダメージ中の解除
        body.SetActive(true); //明確に姿を表示
    }

    //点滅メソッド
    void Blinking()
    {
        float val = Mathf.Sin(Time.time * 50);
        if (val > 0) body.SetActive(true);
        else body.SetActive(false);
    }

    // ギズモで範囲を表示（デバッグ用）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
