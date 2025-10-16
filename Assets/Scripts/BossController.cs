using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("基本設定")]
    public int bossHP = 30;                  // 体力
    public float speed = 3f;                 // 歩行速度
    public float shootSpeed = 15f;           // 弾の速度
    public float moveSpeed = 1.5f;           // タックルの移動速度
    public float fireInterval = 2f;          // 連射間隔
    public float closeRange = 20f;           // 近距離攻撃をする距離
    public float attackInterval = 5f;        // 攻撃間隔
    public GameObject explosionPrefab;  //爆発エフェクト
    private GameObject activeBarrier;   // 展開中のBarrierインスタンス

    [Header("参照オブジェクト")]
    public GameObject bulletPrefab;          // 弾プレハブ
    public GameObject barrier;               // バリア
    public GameObject gate;                  // 弾の生成位置
    public GameObject body;                  //点滅対象
    GameObject player;

    [Header("内部制御")]
    private bool isAttacking;          // 攻撃中かどうか（timer停止用）
    private bool isDamage;             // ダメージ中かどうか
    private bool isInvincible = false; // 無敵時間フラグ
    private bool isShot;               // プレイヤーへ標的を合わせているフラグ
    private float timer = 0f;          // 時間経過

    [Header("SE")]
    AudioSource audioSource;
    public AudioClip SE_Tackle;
    public AudioClip SE_Shot;
    public AudioClip SE_Barrier;
    public AudioClip SE_damage;
    public AudioClip SE_Explosion;

    Rigidbody rbody; //BossのRigidBody

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        gate = GameObject.Find("Gate").gameObject;// Bulletの発射位置

        audioSource = GetComponent<AudioSource>();

        // BossのRigidbodyを初期化し重力や衝突による微妙な動きを止める
        rbody = GetComponent<Rigidbody>();
        if (rbody != null)
        {
            //rbody.isKinematic = true;  // 物理演算を停止
            rbody.useGravity = false;  // 重力無効
        }
    }
    void Update()
    {   //ゲームが停止状態なら動かない
        if (GameManager.gameState != GameState.playing) return;
        if (bossHP <= 0) return;
        if (player == null) return;

        timer += Time.deltaTime;//ゲームの経過時間
        //if (isAttacking) return; // 攻撃中は処理をしない

        // 姿勢補正
        Vector3 euler = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, euler.y, 0);

        // プレイヤーとの距離を常に測定
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // プレイヤーの方向を向く->isShotのフラグが立っていない間はエイムを合わせる
        if (!isShot == true)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime);
        }

        //攻撃中でない時はプレイヤーの方向に歩く
        if (!isAttacking)
        {
            Vector3 moveDir = (player.transform.position - transform.position).normalized;
            moveDir.y = 0; // 上下の動きをさせない
            Vector3 randMoveDir = Random.insideUnitSphere;//ランダムにふらつかせる
            moveDir.x += randMoveDir.x;
            moveDir.z += randMoveDir.z;

            if (rbody != null)
            {
                rbody.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
            }
        }

        // 行動タイマーが満了したら行動開始
        if (timer >= attackInterval)
        {
            Debug.Log("時間が来た");
            if (distance > closeRange)
            {
                // 遠距離 → タックル(0) or ショット(1)をランダムで選択
                int rand = Random.Range(0, 2);
                if (rand == 0)
                    StartCoroutine(TackleCoroutine());
                else
                    StartCoroutine(ShotCoroutine());
            }
            else
            {
                // 近距離 → バリア発動
                StartCoroutine(BarrierCoroutine());
                Debug.Log("Barrierが呼ばれた！");
            }

            timer = 0f;
        }
    }

    // プレイヤーからの攻撃を受けた時のメソッド
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤー弾・剣以外は無視
        if (!other.CompareTag("PlayerBullet") && !other.CompareTag("PlayerSword"))
            return;
        if (isInvincible || bossHP <= 0) return;

        // バリアがONなら弾だけ消してノーダメージ
        if (activeBarrier != null)
        {
            if (other.CompareTag("PlayerBullet")) Destroy(other.gameObject);
            return;
        }

        // バリアがOFFならダメージ判定
        int damage = 0;

        if (other.CompareTag("PlayerBullet"))
        {
            damage = 1;
            Destroy(other.gameObject); // プレイヤーBulletを削除
        }
        else if (other.CompareTag("PlayerSword"))
        {
            damage = 3;
        }

        if (damage > 0)
        {
            // ダメージを受ける
            if (!isInvincible)
            {
                audioSource.PlayOneShot(SE_damage);
                bossHP -= damage;

                // HPが0以下になったら死亡処理
                if (bossHP <= 0)
                {
                    bossHP = 0;
                    audioSource.PlayOneShot(SE_Explosion);

                    //爆発エフェクト生成
                    if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                    Destroy(body);

                    GameManager.gameState = GameState.gameclear;

                    // 1.5秒後にEndingシーンを読み込み
                    Invoke(nameof(LoadEndingScene), 1.5f);
                    return;
                }
                else
                {
                    // ダメージ演出（点滅＋無敵時間）
                    StartCoroutine(DamageFlash());
                }
            }
        }
    }

    //点滅コルーチン：点滅＋点滅中ダメージを受けない無敵処理
    IEnumerator DamageFlash()
    {
        if (bossHP <= 0)
        {
            Destroy(gameObject);
            yield break;
        }

        // 無敵演出と点滅演出（RendererのON・OFFの切り替え）
        isInvincible = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();//複数Rendererを点滅対象にさせる
        for (int i = 0; i < 4; i++)
        {
            foreach (Renderer r in renderers)
                r.enabled = false; // Rendererの表示をOFF
            yield return new WaitForSeconds(0.1f);

            foreach (Renderer r in renderers)
                r.enabled = true;  // Rendererの表示をON
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false;
    }

    // タックルコルーチン：一定時間プレイヤーの方向にLerpで突進
    IEnumerator TackleCoroutine()
    {
        isAttacking = true;

        //Bossのスタート位置
        Vector3 startPos = transform.position;
        //プレイヤーの方向を取得
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        //プレイヤーの少し手前で止まる(逃げる猶予)
        Vector3 targetPos = player.transform.position - dirToPlayer * 1.5f;

        //タックル攻撃
        audioSource.PlayOneShot(SE_Tackle);
        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(1f); // 攻撃の後の余韻

        // タックル終了時に少し後退させる（Barrierが発動しタックルと干渉しないように）
        Vector3 backPos = transform.position - dirToPlayer * 1.0f;
        float backSpeed = 1.5f; // 後退のスピード
        while (Vector3.Distance(transform.position, backPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, backPos, backSpeed * Time.deltaTime);
            yield return null;
        }

        isAttacking = false;
        StartCoroutine(BarrierCoroutine());//バリアコルーチンを呼ぶ
    }

    //ショットコルーチン：一定間隔で球を発射
    IEnumerator ShotCoroutine()
    {
        isAttacking = true;
        isShot = true;

        // 撃つ前に一瞬狙いを定める
        transform.LookAt(player.transform);
        yield return new WaitForSeconds(1f); // 1秒間静止（プレイヤーが避ける余裕）

        int shotCount = 3; // 3連射

        for (int i = 0; i < shotCount; i++)
        {
            //  弾を生成・発射 
            if (bulletPrefab != null && gate != null)
            {
                // Gateの回転にX軸90度だけ回転
                Quaternion bulletRotation = gate.transform.rotation * Quaternion.Euler(90, 0, 0);
                //Bulletの生成
                GameObject bullet = Instantiate(bulletPrefab, gate.transform.position, bulletRotation);

                audioSource.PlayOneShot(SE_Shot);

                // 弾専用のRigidbody
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                if (bulletRb != null)
                {
                    bulletRb.useGravity = false; // 重力オフ
                    bulletRb.linearDamping = 0f;  // 空気抵抗なし
                    bulletRb.angularDamping = 0f; // 回転して減速しないÏ

                    // 発射方向を再取得（プレイヤーが動いた場合に対応）
                    Vector3 dir = (player.transform.position - gate.transform.position).normalized;
                    //Playerの方角へAddForce
                    bulletRb.AddForce(dir * shootSpeed * 10f, ForceMode.Impulse);
                }

                if(bossHP <= 0)
                {
                    Destroy(bulletRb.gameObject);
                }
            }

            // 次の弾までの間隔
            yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(1f); // 最後の発射後の余韻
        isAttacking = false;
        isShot = false;
    }

    //バリアコルーチン: 一定時間展開し外部からの攻撃を弾く
    IEnumerator BarrierCoroutine()
    {

        isAttacking = true;
        Debug.Log("BarrierCoroutine呼ばれた！");


        // Barrierを生成
        if (barrier != null)
        {
            // ボスを覆うようにバリア生成
            activeBarrier = Instantiate(barrier, transform.position + new Vector3(0, 5.0f, 0), Quaternion.identity);
            Debug.Log("Barrier生成成功！");

            audioSource.PlayOneShot(SE_Barrier);

            // バリアがDestroyされる時間分待つ
            yield return new WaitForSeconds(3.0f);

            // 攻撃フラグ解除 
            isAttacking = false;
        }
    }

    // Endingシーンを読み込む
    void LoadEndingScene()
    {
        SceneManager.LoadScene("Ending");
    }
}