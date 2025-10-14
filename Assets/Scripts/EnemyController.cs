using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{    
    public int enemyHP = 5; //敵のHP
    public float enemySpeed = 5.0f; //敵のスピード
    public float enemySlowSpeed = 1.0f; //敵のスピードを緩める

    bool isDamage;　//ダメージ中フラグ
    

    public bool onBullet; //Bulletにあたっているか

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

    float timer; //時間経過

    GameObject gameMgr; //ゲームマネージャー

    public GameObject enemybulletPrefab;
    Transform enemy; //エネミーのtransform情報
    
    public float shootSpeed = 100f; //シュートした時の力
  
    
    public float shootInterval = 2f; //シュートの間隔

    

    bool possibleShoot; //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        //時間差でシュート可能にする
        Invoke("ShootEnabled", 0.5f);

        //エネミーのTransform情報の取得
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
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

        if (isAttack) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectionRange)
        {
            //もしもプレイヤーにある程度近づいたら、近づく速度を緩めてプレイヤーに向かってShot
            if (distance < attackRange)
            {
                enemySpeed = enemySlowSpeed;
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.transform.position);
                // プレイヤーの高さ（Y軸）を無視して、水平に向く
                Vector3 targetPosition = new Vector3(
                    player.transform.position.x,
                    transform.position.y, // 自分の高さを維持
                    player.transform.position.z
                    );
                transform.LookAt(targetPosition);

                //タイマー加算
                timer += Time.deltaTime;

                if (timer > shootInterval)
                {
                    Shot();
                    timer = 0f; //タイマーリセット
                }
               

            }
            else
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.transform.position);
            }
        }
        else
        {
            navMeshAgent.isStopped = true;
        }
        
        
        
    }

        void FixedUpdate()
        {
            //playingモードでないと何もしない
            if (GameManager.gameState != GameState.playing) return;

            //プレイヤーがいない時は何もしない
            if (player == null) return;

            //playerBulletとplayerSwordに触れている時は何もしない
            if (isDamage)
            {


                float val = Mathf.Sin(Time.time * 50);
                if (val > 0)
                {
                    //描画機能を有効
                    GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    //描画機能を無効
                    GetComponent<SpriteRenderer>().enabled = false;
                }

                return;
            }


        }
    


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            if (onBullet) return;

            enemyHP--;
            if (enemyHP > 0)
            {
                onBullet = true;
                StartCoroutine(Damaged());
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }

    IEnumerator Damaged()
    {
        yield return new WaitForSeconds(5);
        onBullet = false;
        //描画機能を有効
        GetComponent<SpriteRenderer>().enabled = true;
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
        if (enemy == null) return;

        //エネミーの位置にenemybulletを生成
        Quaternion rotation = Quaternion.Euler(180f, 0f, 0f);

        GameObject obj = Instantiate(enemybulletPrefab, gate.transform.position, rotation);

        //生成したenemybulletのRigidbodyを取得
        Rigidbody rbody = obj.GetComponent<Rigidbody>();

        //ショットする方向に生成
        Vector3 v = new Vector3(transform.forward.x * shootSpeed, 0, transform.forward.z * shootSpeed);

        rbody.AddForce(v, ForceMode.Impulse);
    }

    // ギズモで範囲を表示（デバッグ用）
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}