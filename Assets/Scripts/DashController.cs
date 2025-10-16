using UnityEngine;
using System.Collections;

public class DashController : MonoBehaviour
{
    // 参照するPlayerControllerとCharacterController
    private PlayerController playerController;
    private CharacterController characterController;

    [Header("Dash Settings")]
    public float dashSpeedMultiplier = 2.0f; // ダッシュ時のスピード倍率（PlayerControllerのmoveSpeedに掛ける）
    public float maxDashStamina = 100f; // ダッシュスタミナの最大値
    public float currentDashStamina; // 現在のダッシュスタミナ（UI表示用など）
    public float dashStaminaConsumptionRate = 30f; // 1秒あたりのスタミナ消費量
    public float dashStaminaRecoveryRate = 20f; // 1秒あたりのスタミナ回復量
    public float dashStaminaRecoveryDelay = 1.0f; // ダッシュ停止から回復開始までの遅延時間

    [Header("Double Press Settings")]
    public float doublePressTimeThreshold = 0.2f; // ダブルクリックと判定する時間

    private bool isDashing = false; // ダッシュ中かどうかのフラグ
    private float lastKeyPressedTime_W;
    private float lastKeyPressedTime_A;
    private float lastKeyPressedTime_S;
    private float lastKeyPressedTime_D;

    private Coroutine staminaRecoveryCoroutine; // スタミナ回復コルーチンへの参照
    private Vector3 currentDashMoveDirection = Vector3.zero; // ダッシュ中の移動方向

    AudioSource audioSource;

    public AudioClip se_Dash;

    void Start()
    {
        currentDashStamina = maxDashStamina; // スタミナを初期化
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //ゲーム状態がプレイ中でなければ何もしない
        if (GameManager.gameState != GameState.playing)
        {
            if (isDashing) StopDash(); // ゲーム停止中はダッシュ状態をリセット
            if (staminaRecoveryCoroutine != null) // スタミナ回復コルーチンも停止
            {
                StopCoroutine(staminaRecoveryCoroutine);
                staminaRecoveryCoroutine = null;
            }
            return;
        }

        HandleDashInput();
        UpdateDashStamina();

        if (isDashing)
        {
            // ダッシュ中はPlayerControllerを無効にする
            if (playerController.enabled)
            {
                playerController.enabled = false;
                // PlayerControllerが最後に設定したy速度を引き継ぐ（ジャンプ中などにダッシュした場合）
                // ただし、PlayerControllerのmoveDirectionはprivateなので、ここでは地面にいると仮定
                currentDashMoveDirection.y = characterController.isGrounded ? 0 : -playerController.gravity * Time.deltaTime; // 簡易的なY軸処理
            }

            // ダッシュ中の移動方向を計算
            float actualMoveSpeed = playerController.moveSpeed * dashSpeedMultiplier; // PlayerControllerのmoveSpeedを利用
            currentDashMoveDirection.x = Input.GetAxisRaw("Horizontal") * actualMoveSpeed;
            currentDashMoveDirection.z = Input.GetAxisRaw("Vertical") * actualMoveSpeed;

            // 重力を加算 (PlayerControllerの重力設定を利用)
            currentDashMoveDirection.y -= playerController.gravity * Time.deltaTime;

            // 移動実行
            Vector3 globalDirection = transform.TransformDirection(currentDashMoveDirection);
            characterController.Move(globalDirection * Time.deltaTime);

            // 地面についていればy成分は0
            if (characterController.isGrounded)
            {
                currentDashMoveDirection.y = 0;
            }
        }
        else
        {
            // ダッシュ中でなければPlayerControllerを有効にする
            if (!playerController.enabled)
            {
                playerController.enabled = true;
            }
        }
    }

    // ダッシュ入力の処理
    void HandleDashInput()
    {
        // Wキーのダブルプレス判定
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastKeyPressedTime_W < doublePressTimeThreshold)
            {
                TryStartDash();
            }
            lastKeyPressedTime_W = Time.time;
        }
        // Aキーのダブルプレス判定
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (Time.time - lastKeyPressedTime_A < doublePressTimeThreshold)
            {
                TryStartDash();
            }
            lastKeyPressedTime_A = Time.time;
        }
        // Sキーのダブルプレス判定
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (Time.time - lastKeyPressedTime_S < doublePressTimeThreshold)
            {
                TryStartDash();
            }
            lastKeyPressedTime_S = Time.time;
        }
        // Dキーのダブルプレス判定
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastKeyPressedTime_D < doublePressTimeThreshold)
            {
                TryStartDash();
            }
            lastKeyPressedTime_D = Time.time;
        }

        // ダッシュ中にいずれかのWASDキーが離された、または移動入力がなくなったらダッシュを停止
        // (ただし、ジャンプ中はダッシュが解除されないように、地面についているかどうかのチェックも追加)
        if (isDashing &&
            (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D)))
        {
            StopDash();
        }
    }

    // ダッシュ開始を試みる
    void TryStartDash()
    {
        // ダッシュ中でなく、かつスタミナが残っている場合
        if (!isDashing && currentDashStamina > 0)
        {
            audioSource.PlayOneShot(se_Dash);
            isDashing = true;
            // ダッシュ開始時はスタミナ回復コルーチンを停止
            if (staminaRecoveryCoroutine != null)
            {
                StopCoroutine(staminaRecoveryCoroutine);
                staminaRecoveryCoroutine = null;
            }
            // PlayerControllerの現在のy速度を初期値として引き継ぐ
            // ただし、PlayerControllerのmoveDirectionはprivateなので、接地状態でのy=0を初期値とする
            currentDashMoveDirection.y = characterController.isGrounded ? 0 : 0; // ここはPlayerControllerのmoveDirection.yにアクセスできないため簡易化
        }
    }

    // ダッシュを停止する
    void StopDash()
    {
        if (isDashing) // ダッシュ中であれば停止処理を実行
        {
            isDashing = false;
            // スタミナ回復コルーチンを開始
            if (staminaRecoveryCoroutine != null)
            {
                StopCoroutine(staminaRecoveryCoroutine);
            }
            staminaRecoveryCoroutine = StartCoroutine(RecoverDashStamina());
        }
    }

    // ダッシュスタミナの更新
    void UpdateDashStamina()
    {
        if (isDashing)
        {
            // ダッシュ中はスタミナを消費
            currentDashStamina -= dashStaminaConsumptionRate * Time.deltaTime;
            if (currentDashStamina <= 0)
            {
                currentDashStamina = 0;
                StopDash(); // スタミナがなくなったらダッシュを強制停止
            }
        }
        // スタミナを0〜最大値にクランプ
        currentDashStamina = Mathf.Clamp(currentDashStamina, 0, maxDashStamina);
    }

    // ダッシュスタミナの回復コルーチン
    IEnumerator RecoverDashStamina()
    {
        yield return new WaitForSeconds(dashStaminaRecoveryDelay); // 回復開始までの遅延

        // ダッシュ中でない間、かつスタミナが最大値に達していない間
        while (currentDashStamina < maxDashStamina && !isDashing)
        {
            currentDashStamina += dashStaminaRecoveryRate * Time.deltaTime;
            currentDashStamina = Mathf.Min(currentDashStamina, maxDashStamina); // 最大値を超えないように
            yield return null; // 1フレーム待つ
        }
        staminaRecoveryCoroutine = null; // コルーチンが終了したら参照をクリア
    }
}