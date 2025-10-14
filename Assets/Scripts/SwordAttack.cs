using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public GameObject swordCollider;
    public GameObject swordPrefab; //ソードのエフェクト
    public float deleteTime = 0.5f;

    bool isAttack; //攻撃中かどうか

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);

        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isAttack)
        {
            isAttack = true;
            StartCoroutine(SwordAttackCoroutine());

            //当たり判定出現
            swordCollider.SetActive(true);
            //エフェクトを生成
            GameObject obj = Instantiate(
                swordPrefab,
                swordCollider.transform.position,
                swordCollider.transform.rotation
                );
            //エフェクトをソードの子オブジェクトにして位置を同期させる
            obj.transform.SetParent(swordCollider.transform);

            //攻撃処理
        }
    }

    //回復時間
    IEnumerator SwordAttackCoroutine()
    {
        //deleteTime後に再度打てるようになる
        yield return new WaitForSeconds(deleteTime);
        //ソードを非表示にしてフラグをfalseにする
        swordCollider.SetActive(false);
        isAttack = false;
    }
}
