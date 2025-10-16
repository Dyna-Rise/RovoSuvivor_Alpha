using UnityEngine;

public class SampleCameraController : MonoBehaviour
{
    // ターゲットとの初期距離の差　（ローカル座標）
    Vector3 diff;
    GameObject player; // ターゲットとなるプレイヤー情報

    public float followSpeed = 8; // カメラの補間スピード

    // カメラの初期位置
    public Vector3 defaultPos = new Vector3(0, 3.5f, -2);
    public Vector3 defaultRotate = new Vector3(20, 0, 0);

    public float mouseSensitivity = 3.0f; //マウス感度

    //上下の角度上限
    public float minVerticalAngle = -15.0f; //下を向く角度限界
    public float maxVerticalAngle = 15.0f; //上を向く角度限界

    //プレイ中のカメラの角度
    float verticalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //画面中心にカーソルをロック
        Cursor.visible = false; //カーソルを非表示

        // カメラを変数で決めた初期位置・角度にする
        transform.position = defaultPos;
        transform.rotation = Quaternion.Euler(defaultRotate);

        // プレイヤー情報の取得
        player = GameObject.FindGameObjectWithTag("Player");

        // diffを初期の差として設定
        diff = player.transform.position - transform.position;

    }

    void LateUpdate()
    {
        //ゲーム状態がプレイ中またはゲームクリア中でなければ何もしない
        if (!((GameManager.gameState == GameState.playing) || (GameManager.gameState == GameState.gameclear))) return;

        // プレイヤーが見つからない場合は何もしない
        if (player == null) return;

        //マウスの動きを取得しておく
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // プレイヤーの左右回転 (Y軸) を更新
        player.transform.Rotate(0, mouseX, 0);

        //その時のマウスの動きに応じた数値（縦方向）
        verticalRotation -= mouseY;
        //最大・最小に絞り込みはされる
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // プレイヤーの現在の位置と回転に基づいて、
        // カメラの目標位置を計算する
        // プレイヤーの回転を考慮したオフセット位置
        Vector3 targetCameraPosition = player.transform.position - player.transform.rotation * diff;

        // 線形補間を使って、カメラを目的の場所に動かす
        // Lerpメソッド(今の位置、ゴールとすべき位置、割合）
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, followSpeed * Time.deltaTime);

        //カメラの目標回転値
        Quaternion playerRotationY = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
        Quaternion cameraRotationX = Quaternion.Euler(verticalRotation, 0, 0);
        Quaternion targetRotation = playerRotationY * cameraRotationX;

        //そのフレームにおけるカメラの角度を最終決定
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}