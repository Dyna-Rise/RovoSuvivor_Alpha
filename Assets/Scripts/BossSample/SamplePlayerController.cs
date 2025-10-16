using UnityEngine;
using System.Collections; // コルーチンのために必要

public class SamplePlayerController : MonoBehaviour
{
    //CharactorControllerコンポーネント
    CharacterController controller;

    public float moveSpeed = 5.0f; //移動スピード
    public float jumpForce = 8.0f; //ジャンプパワー
    public float gravity = 20.0f; //重力

    Vector3 moveDirection = Vector3.zero; //移動成分

    public GameObject body;　//点滅対象
    bool isDamage; //ダメージフラグ

    void Start()
    {
        //CharactorControllerの取得
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //ゲーム状態がプレイ中またはゲームクリア中でなければ何もしない
        if (!((GameManager.gameState == GameState.playing) || (GameManager.gameState == GameState.gameclear))) return;

        // ダメージ中の点滅処理
        if (isDamage)
        {
            Blinking();
        }

        //地面の上にいる場合
        if (controller.isGrounded)
        {

            //　水平入力
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                moveDirection.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
            }
            else
            {
                moveDirection.x = 0;
            }

            // 垂直入力
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                moveDirection.z = Input.GetAxisRaw("Vertical") * moveSpeed;
            }
            else
            {
                moveDirection.z = 0;
            }

            // ジャンプ入力
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        //重力を加算
        moveDirection.y -= gravity * Time.deltaTime;

        // 移動実行
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        //地面についていればy成分は0
        if (controller.isGrounded) moveDirection.y = 0;
    }

    //接触したら
    private void OnTriggerEnter(Collider other)
    {
        //敵本体か、敵の弾に当たったら
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Barrier"))
        {
            //ダメージ中なら何もしない
            if (isDamage) return;

            isDamage = true; //ダメージ中
            GameManager.playerHP--; //プレイヤーHP現象

            if (GameManager.playerHP <= 0) //プレイヤーHPがなくなったら
            {
                //ゲームオーバーへ
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 1.0f);
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
}