using UnityEngine;

public class PlayerAnime : MonoBehaviour
{
    public Animator animator; // Animatorコンポーネント
    bool isDeadAnime; //死亡アニメが未実行かどうか

    void Update()
    {
        //プレイ中でなければ
        if (GameManager.gameState != GameState.playing)
        {
            //ゲームオーバー状態ならDeadアニメを一度発動させる
            if (GameManager.gameState == GameState.gameover)
            {
                if (!isDeadAnime)
                {
                    DeadAnimation();
                }
            }
            return; //プレイ中じゃない時点で後は何もしない
        }

        MoveAnimation(); // 移動アニメーション
        AttackAnimation();   // 攻撃アニメーション
        JumpAnimation();   // ジャンプアニメーション

    }

    //移動アニメ
    void MoveAnimation()
    {
        //まずはWalkフラグをOFF
        bool isMoving = false;

        //左右キーが入ったらそれぞれの方向
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            animator.SetInteger("direction", 3);
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            animator.SetInteger("direction", 1);
            isMoving = true;
        }

        //上下キーが入ったらそれぞれの方向
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            animator.SetInteger("direction", 0);
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            animator.SetInteger("direction", 2);
            isMoving = true;
        }

        //入力状況に応じてWalkフラグが立つか立たないか
        animator.SetBool("walk", isMoving);

    }

    //攻撃アニメ
    void AttackAnimation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("shot");
        }
    }

    //ジャンプアニメ
    void JumpAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
        }
    }

    //死亡アニメ
    void DeadAnimation()
    {
        animator.SetTrigger("die");
        isDeadAnime = true;
    }
}