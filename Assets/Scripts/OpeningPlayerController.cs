using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class OpeningPlayerController : MonoBehaviour
{
    public GameObject[] points;
    Vector3 targetPos;

    NavMeshAgent agent;
    int countPoint = 0;

    bool isChange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targetPos = points[countPoint].transform.position;
        agent.SetDestination(targetPos);
    }

    private void Update()
    {
        Debug.Log(countPoint);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Point"))
        {
            Debug.Log("接触");
            if (!isChange) {
                StartCoroutine(NextPoint());
            }
        }
    }

    IEnumerator NextPoint()
    {
        isChange = true;
        countPoint++;
        if (countPoint >= points.Length)
        {
            countPoint = 0;
        }
        targetPos = points[countPoint].transform.position;
        agent.SetDestination(targetPos);

        yield return new WaitForSeconds(3);
        isChange = false;

    }

}
