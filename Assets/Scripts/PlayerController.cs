using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
   
    CharacterController controller;

    public float moveSpeed = 5.0f; //移動スピード
    public float jumpForce = 8.0f; //ジャンプパワー
    public float gravity = 20.0f; //重力

    Vector3 moveDirection = Vector3.zero; //移動成分

    public GameObject body;　//点滅対象
    bool isDamage; //ダメージフラグ

    AudioSource audioSource;

    public AudioClip se_Walk;
    public AudioClip se_Damage;
    public AudioClip se_Explosion;
    public AudioClip se_Jump;

    float walkInterval = 0.6f;
    float walkTimer;


    void Start()
    {
        
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

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
                audioSource.PlayOneShot(se_Jump);
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

  
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);


        if (controller.isGrounded) moveDirection.y = 0;

        walkSe();


    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet") || other.gameObject.CompareTag("Barrier"))
        {
           
            if (isDamage) return;

            isDamage = true;
            GameManager.playerHP--;
            if (audioSource != null && se_Damage != null)
            {
                audioSource.PlayOneShot(se_Damage);
            }



            if (GameManager.playerHP <= 0)
            {
                audioSource.PlayOneShot(se_Explosion);
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

    void walkSe()
    {
        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            walkTimer += Time.deltaTime; 

            if (walkTimer >= walkInterval) 
            {
                audioSource.PlayOneShot(se_Walk);
                walkTimer = 0;
            }
        }
        else 
        {
            walkTimer = 0f;
        }
    }
}