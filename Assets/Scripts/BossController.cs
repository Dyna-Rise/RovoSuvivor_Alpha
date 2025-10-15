using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("基本設定")]
    public int bossHP = 30;                  // 体力
    public float speed = 3f;                 // 移動速度
    public float shootSpeed = 15f;           // 弾の速度
    public float moveSpeed = 5f;             // タックルの移動速度
    public float fireInterval = 2f;          // 連射間隔
    public float closeRange = 3f;            // 近距離攻撃をする距離
    public float attackInterval = 5f;        // 攻撃間隔

    [Header("参照オブジェクト")]
    public GameObject bulletPrefab;          // 弾プレハブ
    public GameObject barrier;               // バリア
    public GameObject gate;                  // 弾の生成位置
    public GameObject body;                  //点滅対象
    GameObject player;

    [Header("内部制御")]
    private bool isAttacking;     // 攻撃中かどうか（timer停止用）
    private bool isDamage;     // ダメージ中かどうか
    private bool isInvincible = false; // 無敵時間フラグ
    private float timer = 0f;          // 時間経過

    Rigidbody rbody;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        gate = GameObject.Find("Gate");             // 弾の発射位置
        barrier = GameObject.FindGameObjectWithTag("Barrier");
        barrier.SetActive(false); //ゲーム開始時はバリアを非表示にしておく
    }
    void Update()
    {   //ゲームが停止状態なら動かない
        if (GameManager.gameState != GameState.playing) return;
        //プレイヤーがいないなら何もしない
        if (player == null) return;

        if (isAttacking) return; // コルーチン中はtimer停止

        timer += Time.deltaTime;

        // プレイヤーの方向を向く
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 3f * Time.deltaTime);

        // 行動タイマーが満了したら行動開始
        if (timer >= attackInterval)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > closeRange)
            {
                // 遠距離 → タックル or ショットをランダムで選択
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
            }

            timer = 0f;
        }
    }

    // プレイヤーからの攻撃を受けた時のメソッド
    private void OnTriggerEnter(Collider other)
    {
        if (isInvincible || bossHP <= 0) return;

        // バリアがONなら弾だけ消してノーダメージ
        if (barrier != null && barrier.activeSelf)
        {
            if (other.CompareTag("PlayerBullet") || other.CompareTag("PlayerSword"))
            {
                Destroy(other.gameObject);
                return;
            }
        }

        // バリアがOFFならダメージ判定
        int damage = 0;

        if (other.CompareTag("PlayerBullet"))
        {
            damage = 1;
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
                bossHP -= damage;

                // HPが0以下になったら死亡処理
                if (bossHP <= 0)
                {
                    bossHP = 0;
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

            // 弾を削除
            Destroy(other.gameObject);
        }
    }

    // Endingシーンを読み込む
    void LoadEndingScene()
    {
        SceneManager.LoadScene("Ending");
    }

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
                r.enabled = false; // ← Rendererの表示をOFF
            yield return new WaitForSeconds(0.1f);

            foreach (Renderer r in renderers)
                r.enabled = true;  // ← Rendererの表示をON
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false;

    }

    // タックルコルーチン：一定時間プレイヤーの方向にLerpで突進
    IEnumerator TackleCoroutine()
    {
        isAttacking = true;
        Vector3 startPos = transform.position;
        Vector3 targetPos = player.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            t += Time.deltaTime * speed;
            yield return null;
        }

        yield return new WaitForSeconds(1f); // 余韻
        isAttacking = false;
    }

    //ショットコルーチン：一定間隔で球を発射
    IEnumerator ShotCoroutine()
    {
        isAttacking = true;

        // 撃つ前に一瞬狙いを定める（演出用）
        transform.LookAt(player.transform);
        yield return new WaitForSeconds(1f); // 1秒間静止（プレイヤーが避ける余裕）

        int shotCount = 3; // 3連射

        for (int i = 0; i < shotCount; i++)
        {
            // ===== 弾を生成・発射 =====
            if (bulletPrefab != null && gate != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, gate.transform.position, gate.transform.rotation);

                rbody = bullet.GetComponent<Rigidbody>();
                if (rbody != null)
                {
                    // 発射方向を再取得（プレイヤーが動いた場合に対応）
                    Vector3 dir = (player.transform.position - gate.transform.position).normalized;
                    rbody.AddForce(dir * shootSpeed, ForceMode.Impulse);
                }
            }

            // ===== 次の弾までの間隔 =====
            yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(1f); // 最後の発射後の余韻
        isAttacking = false;
    }

    //バリアコルーチン、一定時間展開し外部からの攻撃を弾く
    IEnumerator BarrierCoroutine()
    {
        isAttacking = true;

        // シーンに配置済みのbarrierをONに
        if (barrier != null)
            barrier.SetActive(true);

        float duration = 3f; // バリアの持続時間
        yield return new WaitForSeconds(duration);

        // 時間が経ったらOFFに戻す
        if (barrier != null)
            barrier.SetActive(false);

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
}

