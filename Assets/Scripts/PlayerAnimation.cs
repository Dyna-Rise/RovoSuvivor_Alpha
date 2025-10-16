using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    bool isDeadAnime;
    CharacterController controller;

    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        if (controller == null) Debug.LogWarning("CharacterController ‚ªeŠK‘w‚ÉŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
    }


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
       
        if (controller != null && controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
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