using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float pushForce = 5.0f;

    public float deleteTime = 3.0f;

    private void Start()
    {
        Destroy(gameObject,deleteTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterController characterCnt = other.GetComponent<CharacterController>();

            Vector3 pushDirection = (other.transform.position - transform.position).normalized;

            
            pushDirection.y = 0;

            // �����o���x�N�g�����v�Z
            Vector3 moveVector = pushDirection * pushForce * Time.deltaTime;

            // �����CharacterController���ړ�������
            characterCnt.Move(moveVector);
        }
    }
}
