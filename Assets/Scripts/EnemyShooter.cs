using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    public GameObject enemybulletPrefab;
    Transform enemy; //エネミーのtransform情報
    GameObject gate; //エネミーについているGateオブジェクトの情報
    public float shootSpeed = 100f; //シュートした時の力
    public float enemySpeed = 5.0f; //敵のスピード
    public float enemySlowSpeed = 1.0f; //敵のスピードを緩める

    public float shootInterval = 2f; //シュートの間隔

    private float timer = 0;

    bool possibleShoot; //

    private NavMeshAgent navMeshAgent;     // NavMeshAgentコンポーネント
    GameObject player;


    // ヒットポイント

    public float stopRange = 8f;




    void Start()
    {
        //時間差でシュート可能にする
        Invoke("ShootEnabled", 0.5f);

        //エネミーのTransform情報の取得
        enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        //エネミーについているGateオブジェクト情報の取得
        gate = enemy.Find("Gate").gameObject;


    }


    void Update()
    {
        if (GameManager.gameState != GameState.playing) return;

        
        //playingモードでないと何もしない
        if (GameManager.gameState != GameState.playing) return;

        //プレイヤーがいない時は何もしない
        if (player == null) return;


        float distance = Vector3.Distance(transform.position, player.transform.position);

        //もしもプレイヤーにある程度近づいたら、近づく速度を緩めてプレイヤーに向かってShot
        if (distance < stopRange)
        {
            enemySpeed = enemySlowSpeed;

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
        GameObject obj = Instantiate(enemybulletPrefab, gate.transform.position, Quaternion.identity);

        //生成したenemybulletのRigidbodyを取得
        Rigidbody rbody = obj.GetComponent<Rigidbody>();

        //ショットする方向に生成
        Vector3 v = new Vector3(transform.forward.x * shootSpeed, 0, transform.forward.z * shootSpeed);

        rbody.AddForce(v, ForceMode.Impulse);
    }
}
