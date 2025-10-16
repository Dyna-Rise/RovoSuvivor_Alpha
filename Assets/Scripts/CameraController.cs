using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player; //プレイヤー
    //float diff; //プレイヤーとの距離
    Vector3 diff; //プレイヤーとの距離

    [SerializeField] float followSpeed = 8f; //カメラの補間スピード

    //カメラの初期位置（プレイヤー位置のY + 2.5, Z -2.0当たりにするとおおむねUI照準通りに弾が飛ぶ）
    [SerializeField] Vector3 defaultPos = new Vector3(0, 3.5f, -2);
    //[SerializeField] Vector3 defaultRotate = new Vector3(15, 0, 0);
    [SerializeField] Vector3 defaultRotate = new Vector3(20, 0, 0);

    [SerializeField] float mouseSensitivity = 3.0f; //マウス感度

    //カメラ角度の上限
    [SerializeField] float minVerticalAngle = -15.0f; //上向き
    [SerializeField] float maxVerticalAngle = 15.0f; //下向き

    //カメラの角度
    float verticalRotation = 0;

    private void Start()
    {
        //カーソルを非表示
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //カメラを初期位置、角度にセット
        transform.position = defaultPos;
        transform.rotation = Quaternion.Euler(defaultRotate);

        //プレイヤーとカメラとの距離を記録
        player = GameObject.FindGameObjectWithTag("Player");
        //diff = Vector3.Distance(player.transform.position, transform.position);
        diff = player.transform.position - transform.position;
    }

    private void LateUpdate()
    {
        if (GameManager.gameState != GameState.playing) return;
        if (player == null) return;

        //マウスの動きを取得
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //プレイヤーの左右回転
        player.transform.Rotate(new Vector3(0, mouseX, 0));

        //縦方向(マイナスにして動かしやすく)
        verticalRotation = Mathf.Clamp(verticalRotation - mouseY, minVerticalAngle, maxVerticalAngle);

        //角度をラジアンに変換
        //float playerRotationY = player.transform.eulerAngles.y * Mathf.Deg2Rad;
        //プレイヤーの位置からdiffだけ離れた位置にカメラを移動
        //Vector3 targetCameraPosition = new Vector3(
        //    player.transform.position.x - Mathf.Sin(playerRotationY) * diff,
        //    defaultPos.y,
        //    player.transform.position.z - Mathf.Cos(playerRotationY) * diff
        //    );

        // プレイヤーの現在の位置と回転に基づいて、
        // カメラの目標位置を計算する
        // プレイヤーの回転を考慮したオフセット位置
        Vector3 targetCameraPosition = player.transform.position - player.transform.rotation * diff;

        //カメラの位置を決定
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, followSpeed * Time.deltaTime);

        //カメラの角度
        Quaternion targetRotation =
            Quaternion.Euler(0, player.transform.eulerAngles.y, 0) *
            Quaternion.Euler(verticalRotation, 0, 0);
        //カメラの角度を決定
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
    }
}
