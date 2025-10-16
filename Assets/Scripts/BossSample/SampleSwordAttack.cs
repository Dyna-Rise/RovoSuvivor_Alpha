using System.Collections;
using UnityEngine;

public class SampleSwordAttack : MonoBehaviour
{
    public GameObject swordCollider;
    public GameObject swordPrefab;
    public float deleteTime = 0.5f;

    bool isAttack;

    void Start()
    {
        swordCollider.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (!isAttack)
            {
                StartCoroutine(SwordAttackStart());
            }            
        }
    }

    IEnumerator SwordAttackStart()
    {
        isAttack = true;
        swordCollider.SetActive (true);
        GameObject obj = Instantiate(
            swordPrefab,
            transform.position + new Vector3(0,1.5f,0),
            Quaternion.Euler(transform.eulerAngles.x - 10, transform.eulerAngles.y - 90,0));
        obj.transform.SetParent(transform);
        yield return new WaitForSeconds(deleteTime);
        swordCollider.SetActive(false);
        isAttack = false;
    }
}
