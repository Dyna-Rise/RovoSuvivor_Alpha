using UnityEngine;

public class DieEffect : MonoBehaviour
{
    public float deleteTime = 3.0f;

    void Start()
    {
        Destroy(gameObject,deleteTime);
    }
}
