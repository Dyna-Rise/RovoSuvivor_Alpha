using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public GameObject swordCollider;
    public GameObject swordPrefab; //ソードのエフェクト
    public float deleteTime = 0.5f;

    bool isAttack; //攻撃中かどうか

    AudioSource audioSource;
    [SerializeField] AudioClip se_sword;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
            Attack();
        }
    }

    //攻撃メソッド
    void Attack()
    {
        //攻撃フラグを立てて回復時間の計測を開始
        isAttack = true;
        StartCoroutine(SwordAttackCoroutine());

        //アタック音を鳴らす
        audioSource.PlayOneShot(se_sword);

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

        //ダメージ処理はエネミー側で実行
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
