using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
   
    CharacterController controller;

    public float moveSpeed = 5.0f; //�ړ��X�s�[�h
    public float jumpForce = 8.0f; //�W�����v�p���[
    public float gravity = 20.0f; //�d��

    Vector3 moveDirection = Vector3.zero; //�ړ�����

    public GameObject body;�@//�_�őΏ�
    bool isDamage; //�_���[�W�t���O


    void Start()
    {
        
        controller = GetComponent<CharacterController>();

    }

    void Update()
    {
       
        if (!((GameManager.gameState == GameState.playing) || (GameManager.gameState == GameState.gameclear))) return;

       
        if (isDamage)
        {
            Blinking();
        }

        if (controller.isGrounded)
        {

          
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                moveDirection.x = Input.GetAxisRaw("Horizontal") * moveSpeed;
            }
            else
            {
                moveDirection.x = 0;
            }

           
            if (Input.GetAxisRaw("Vertical") != 0)
            {
                moveDirection.z = Input.GetAxisRaw("Vertical") * moveSpeed;
            }
            else
            {
                moveDirection.z = 0;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

  
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);


        if (controller.isGrounded) moveDirection.y = 0;


    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Barrier"))
        {

            if (isDamage) return;

            isDamage = true;
            GameManager.playerHP--;


            if (GameManager.playerHP <= 0)
            {
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 1.0f);
            }


            StartCoroutine(DamageReset());
        }
    }

    IEnumerator DamageReset()
    {
        yield return new WaitForSeconds(1.0f);

        isDamage = false;
        body.SetActive(true);
    }

    void Blinking()
    {
        float val = Mathf.Sin(Time.time * 50);
        if (val > 0) body.SetActive(true);
        else body.SetActive(false);
    }
}