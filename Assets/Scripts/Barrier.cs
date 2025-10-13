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

            // Y軸方向の力は通常は加えない（地面にめり込んだり、浮き上がったりしないように）
            // 必要であれば調整してください
            pushDirection.y = 0;

            // 押し出すベクトルを計算
            Vector3 moveVector = pushDirection * pushForce * Time.deltaTime;

            // 相手のCharacterControllerを移動させる
            characterCnt.Move(moveVector);
        }
    }
}
