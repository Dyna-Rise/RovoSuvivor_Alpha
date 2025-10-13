using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float deleteTime = 3.0f;

    void Start()
    {
        Destroy(gameObject,deleteTime);
    }
}
