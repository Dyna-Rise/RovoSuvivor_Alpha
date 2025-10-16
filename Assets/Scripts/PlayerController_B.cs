using UnityEngine;

public class PlayerController_B : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] float speed = 3.0f;
    [SerializeField] float jumpSpeed = 6.0f;
    [SerializeField] int hp = 10;

    Vector3 moveDirection = Vector3.zero;

    const float gravity = -9.81f;
    float moveX;
    float moveZ;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveZ = Input.GetAxisRaw("Vertical");


        //左右
        if (moveX > 0)
        {
            controller.Move(transform.right * speed * Time.deltaTime);
        }
        else if (moveX < 0)
        {
            controller.Move(-transform.right * speed * Time.deltaTime);
        }
        //前後
        if (moveZ > 0)
        {
            controller.Move(transform.forward * speed * Time.deltaTime);
        }
        else if (moveZ < 0)
        {
            controller.Move(-transform.forward * speed * Time.deltaTime);
        }
        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            Invoke("Jump", 0.3f);
        }

        moveDirection.y += gravity * Time.deltaTime;
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        //移動後接地してたらY方向の速度はリセットする
        if (controller.isGrounded) moveDirection.y = 0;
    }

    void Jump()
    {
        moveDirection.y += jumpSpeed;
    }

    public void TakeDamage()
    {
        hp--;
    }
}
