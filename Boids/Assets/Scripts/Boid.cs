using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    private Collider[] detectNeighborList;

    public List<Transform> detectList = new List<Transform>();
    public List<Transform> separationList = new List<Transform>();
    public Vector3 egoNormalVector = Vector3.zero;

    [Range(1f, 360f), SerializeField] private float viewAngle;
    
    private void Start()
    {
        detectNeighborList = new Collider[BoidsSpawner.Instance.boidsMaxCount];
        GetNeighbor();
        StartCoroutine(StartGetNeighbor());
        StartCoroutine(SetEgoVector());
    }

    private IEnumerator StartGetNeighbor()
    {
        while(true)
        {
            GetNeighbor();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    private IEnumerator SetEgoVector()
    {
        while(true)
        {
            egoNormalVector = Random.insideUnitSphere;
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    private void GetNeighbor()
    {
        detectList.Clear();
        separationList.Clear();

        //��ä�� ���·� �����ϵ��� ����

        int count = Physics.OverlapSphereNonAlloc(transform.position, BoidsSpawner.Instance.detectRadius, detectNeighborList);
        for (int i = 0; i < count; i++)
        {
            Vector3 toTarget = (detectNeighborList[i].transform.position - transform.position).normalized;
            float cosThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

            if (detectNeighborList[i].gameObject == this.gameObject) continue;
            if (Vector3.Dot(transform.forward, toTarget) < cosThreshold) continue;
            if (Vector3.Distance(transform.position, detectNeighborList[i].transform.position) <= BoidsSpawner.Instance.separationRadius)
            {
                separationList.Add(detectNeighborList[i].transform);
                detectList.Add(detectNeighborList[i].transform);
            }

            else
            {
                detectList.Add(detectNeighborList[i].transform);
            }

        }
    }
}
