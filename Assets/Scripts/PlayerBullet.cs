using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float deleteTime = 3.0f;

    void Start()
    {
        Destroy(gameObject,deleteTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
