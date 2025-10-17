using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float deleteTime = 3.0f;
    public float deleteColliderTime = 0.7f;

    void Start()
    {
        Invoke("DestroyCollider", deleteColliderTime);
        Destroy(gameObject,deleteTime);
    }

    void DestroyCollider()
    {
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
    }
}
