using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    Vector3 moveDirection;
    CharacterController characterCnt;
    public float speed = 5.0f;

    private void Start()
    {
        characterCnt = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveDirection.x = Input.GetAxisRaw("Horizontal");
        moveDirection.z = Input.GetAxisRaw("Vertical");

        moveDirection.y -= 9.81f * Time.deltaTime;
        characterCnt.Move(moveDirection * speed * Time.deltaTime);

        if (characterCnt.isGrounded)
        {
            moveDirection.y = 0;
        }

    }
}