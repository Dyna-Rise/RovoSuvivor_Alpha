using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Animator animator; //Animatorコンポーネントを扱うための変数

    public int enemyHP = 5; //敵のHP
    public float enemySpeed = 5.0f; //敵のスピード
    public float enemySlowSpeed = 2.5f; //敵のスピードを緩める

    bool isDamage; //ダメージ中フラグ



    public GameObject body; //点滅されるbody

    GameObject player;      // プレイヤーのTransformをInspectorから設定
    NavMeshAgent navMeshAgent;     // NavMeshAgentコンポーネンス

    public float detectionRange = 80f;     // プレイヤーを検知する距離

    bool isAttack; //攻撃中フラグ
    public float attackRange = 30f;         // 攻撃を開始する距離
    public float stopRange = 5f; //接近限界距離
    public GameObject bulletPrefab;     // 発射する弾のPrefab
    public GameObject gate;            // 弾を発射する位置
    public float bulletSpeed = 100f;    // 発射する弾の速度 
    public float fireInterval = 2.0f; //弾を発射するインターバル
    bool lockOn = true; //ターゲット

    const float DamageDuration = 0.5f;
    float recoverTime = 0.0f;

    float timer; //時間経過

    GameObject gameMgr; //ゲームマネージャー

    public GameObject enemybulletPrefab;
    Transform enemy; //エネミーのtransform情報

    public float shootSpeed = 100f; //シュートした時の力


    public float shootInterval = 2f; //シュートの間隔

    bool possibleShoot; //シュートを可能とする

    public GameObject flame; //炎のエフェクト

    //音にまつわるコンポーネントとSE音情報
    AudioSource audio;
    public AudioClip se_shot;
    public AudioClip se_damage;
    public AudioClip se_explosion;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //animator = GetComponent<Animator>();//Animatorコンポーネントの情報を代入
        audio = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        //GameManager取得
        gameMgr = GameObject.Find("GameManager");

        //時間差でシュート可能にする
        Invoke("ShootEnabled", 0.5f);

        //エネミーのTransform情報の取得
        enemy = transform;
        //エネミーについているGateオブジェクト情報の取得
        gate = enemy.Find("Gate").gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        //playingモードでないと何もしない
        if (GameManager.gameState != GameState.playing) return;

        //プレイヤーがいない時は何もしない
        if (player == null) return;

        //もしisDamageフラグ中なら点滅
        if (IsDamage())
        {

            //復活までの時間をカウント
            recoverTime -= Time.deltaTime;

            //点滅処理
            Blinking();
            return;
        }

        //攻撃中なら何もしない
        if (isAttack) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange)
        {
            //もしもプレイヤーにある程度近づいたら、近づく速度を緩めてプレイヤーに向かってShot
            if (distance < attackRange)
            {
                navMeshAgent.speed = enemySlowSpeed; //減速させる
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.transform.position);

                if (lockOn)
                {
                    // プレイヤーの高さ（Y軸）を無視して、水平に向く
                    Vector3 targetPosition = new Vector3(
                        player.transform.position.x,
                        transform.position.y, // 自分の高さを維持
                        player.transform.position.z
                        );
                    transform.LookAt(targetPosition);
                }

                //タイマー加算
                timer += Time.deltaTime;

                if (timer > shootInterval)
                {
                    Shot();
                    timer = 0f; //タイマーリセット
                }

                //近接限界処理になったらEnemyを止める
                if (distance < stopRange)
                {
                    navMeshAgent.isStopped = true; //Enemyを止める
                }


            }
            else
            {
                navMeshAgent.speed = enemySpeed; //元の速度に戻す
                navMeshAgent.isStopped = false; //Enemyを動かす
                navMeshAgent.SetDestination(player.transform.position); //playerを目的地とする

            }
        }
        else
        {
            navMeshAgent.isStopped = true; //Enemyを止める
        }



    }

    void FixedUpdate()
    {
        //playingモードでないと何もしない
        if (GameManager.gameState != GameState.playing) return;

        //プレイヤーがいない時は何もしない
        if (player == null) return;


    }



    private void OnTriggerEnter(Collider collision)
    {
        if (IsDamage()) return;
        ////EnemyHPが0なら何もしない
        //if (enemyHP <= 0) return;

        //PlayerBulletに当たったら-1ダメージ
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            //体力をマイナス
            enemyHP--;

            SEPlay(SEType.Damage); //ダメージ音を鳴らす

            //enemyHPが0より多ければDamageフラグをtrueにしてDamageメソッド発動
            if (enemyHP <= 0)
            {
                SEPlay(SEType.Explosion); //爆発音を鳴らす
                Die();
                return;
            }
            //recoverTimeの時間を設定
            recoverTime = DamageDuration;
            isDamage = true;

        }
        //PlayerSwordに当たったら3倍ダメージ
        else if (collision.gameObject.CompareTag("PlayerSword"))
        {
            enemyHP -= 3;

            SEPlay(SEType.Damage); //ダメージ音を鳴らす

            //enemyHPが0でDieメソッド発動
            if (enemyHP <= 0)
            {
                SEPlay(SEType.Explosion); //爆発音を鳴らす
                Die();
                return;

            }
            //recoverTimeの時間を設定
            recoverTime = DamageDuration;
            isDamage = true;
        }
    }

    bool IsDamage()
    {
        //Lifeが0になった場合はisDamageフラグがON
        bool damage = recoverTime > 0.0f;
        //isDamageフラグがOFFの場合はボディを確実に表示
        if (!damage) body.SetActive(true);
        //Damageフラグをリターン
        return damage;
    }

    //点滅処理
    void Blinking()
    {
        //その時のゲーム進行時間で正か負かの値を算出
        float val = Mathf.Sin(Time.time * 50);
        //正の周期なら表示
        if (val >= 0) body.SetActive(true);
        //負の周期なら非表示
        else body.SetActive(false);


    }

    //ショット可能にする
    void ShootEnabled()
    {
        possibleShoot = true;
    }

    //ショットメソッド
    void Shot()
    {
        //エネミーが消滅していなければ
        if (enemy == null || enemyHP <= 0) return;

        SEPlay(SEType.Shot); //ショット音を鳴らす

        isAttack = true;
        lockOn = false;

        //bulletPrefabのxを90度調整
        Quaternion rotation = Quaternion.Euler(90, 0, 0);

        //エネミーの子オブジェクトであるgateの位置にenemybulletを生成
        GameObject obj = Instantiate(enemybulletPrefab, gate.transform.position, gate.transform.rotation * rotation);

        //生成したenemybulletのRigidbodyを取得
        Rigidbody rbody = obj.GetComponent<Rigidbody>();

        //ショットする方向に生成
        Vector3 v = new Vector3(transform.forward.x * shootSpeed, 0, transform.forward.z * shootSpeed);

        rbody.AddForce(v, ForceMode.Impulse);

        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(fireInterval);
        isAttack = false;
        lockOn = true;
        timer = 0f;
    }


    void Die()
    {
        //animator.SetBool("Dead", true); //デッドアニメに切り替え
        animator.SetTrigger("die");
        // GameManagerからenemyListを取得し、先頭の要素を削除
        if (gameMgr == null)
        {
            gameMgr = GameObject.Find("GameManager");
        }

        GameManager gm = gameMgr.GetComponent<GameManager>();
        if (gm != null && gm.enemyList != null && gm.enemyList.Count > 0)
        {

            gm.enemyList.RemoveAt(0); //先頭を削除
        }


        Destroy(gameObject, 1); //Enemyオブジェクト削除
        Instantiate(
               flame, //生成したいオブジェクト
               this.transform.position, //Enemyの位置
               flame.transform.rotation
               );
    }


    // ギズモで範囲を表示（デバッグ用）
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, stopRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    //SE再生
    public void SEPlay(SEType type)
    {
        switch (type)
        {
            case SEType.Shot:
                audio.PlayOneShot(se_shot);
                break;
            case SEType.Damage:
                audio.PlayOneShot(se_damage);
                break;
            case SEType.Explosion:
                audio.PlayOneShot(se_explosion);
                break;
        }
    }
}