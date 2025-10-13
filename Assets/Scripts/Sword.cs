using UnityEngine;

public class Sword : MonoBehaviour
{
    public float deleteTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, deleteTime);
    }

}
