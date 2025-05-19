using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Sort : MonoBehaviour
{
    public abstract IEnumerator ExecuteSort(List<SortElement> sortList, float speed);

    protected void SetColor(GameObject gameObject, Color color)
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer != null) renderer.material.color = color;
    }

    protected void Swap(List<SortElement> sortList, int a, int b)
    {
        SortElement temp = sortList[a];
        sortList[a] = sortList[b];
        sortList[b] = temp;

        Vector3 posA = sortList[a].prefab.transform.localPosition;
        Vector3 posB = sortList[b].prefab.transform.localPosition;

        sortList[a].prefab.transform.localPosition = posB;
        sortList[b].prefab.transform.localPosition = posA;
    }
}
