using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    private Collider[] detectNeighborList = new Collider[100];

    public List<Transform> detectList = new List<Transform>();
    public List<Transform> separationList = new List<Transform>();


    private void Start()
    {
        GetNeighbor();
        StartCoroutine(StartGetNeighbor());
    }

    private IEnumerator StartGetNeighbor()
    {
        while(true)
        {
            GetNeighbor();
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }

    private void GetNeighbor()
    {
        detectList.Clear();
        separationList.Clear();

        int count = Physics.OverlapSphereNonAlloc(transform.position, BoidsSpawner.Instance.detectRadius, detectNeighborList);
        for (int i = 0; i < count; i++)
        {
            if (detectNeighborList[i].gameObject == this.gameObject) continue;

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
