using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    bool isDeadAnime;

    void Update()
    {      
        if (GameManager.gameState != GameState.playing)
        {
            if (GameManager.gameState == GameState.gameover)
            {
                if (!isDeadAnime)
                {
                    DeadAnimation();
                }
            }
            return;
        }

        MoveAnimation(); 
        AttackAnimation();  
        JumpAnimation();  
    }

    void MoveAnimation()
    {
        bool isMoving = false;
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

        animator.SetBool("walk", isMoving);

    }

    void AttackAnimation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("shot");
        }
    }

    void JumpAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
        }
    }

    void DeadAnimation()
    {
        animator.SetTrigger("die");
        isDeadAnime = true;
    }
}