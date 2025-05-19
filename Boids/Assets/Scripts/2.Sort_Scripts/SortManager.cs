using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//모든 오브젝트 list 관리하는 곳
//Sort에서 오브젝트 위치 및 관리
//그 밑 cs에서 알고리즘 계산하기
[System.Serializable]
public class SortElement
{
    public GameObject prefab;
    public float value;

    public SortElement(GameObject prefab, float value)
    {
        this.prefab = prefab;
        this.value = value;
    }

    public void PlayAudio()
    {
        prefab.GetComponent<AudioSource>().Play();
    }
}

public class SortManager : MonoBehaviour
{
    [Serializable]
    private struct SortStruct
    {
        public SortType type;
        public Sort sort;
    }

    [SerializeField] private List<SortStruct> sortStructs;
    [SerializeField] private TextMeshProUGUI sortName;
    [SerializeField] private GameObject sortPrefab;

    [Range(0.01f, 1f), SerializeField] private float sortSpeed;
    [Range(0.1f, 2f), SerializeField] private float sortSpace;
    [Range(50, 1000), SerializeField] private int sortCount;

    [SerializeField] private SortType sortType;

    private List<SortElement> currentSortList = new List<SortElement>();
    private Dictionary<SortType, Sort> sortDictionary;
    private bool isSorting = false;

    private void Awake()
    {
        sortDictionary = new Dictionary<SortType, Sort>();
        foreach(SortStruct component in sortStructs)
        {
            if (component.sort != null) sortDictionary[component.type] = component.sort;
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        DiscardObjects();
        InstantiateObjects();
        ExecuteSort();
    }

    private void DiscardObjects()
    {
        foreach(SortElement element in currentSortList)
        {
            Destroy(element.prefab);
        }
        currentSortList.Clear();
    }

    private void InstantiateObjects()
    {
        List<float> randomList = new List<float>();
        Vector3 origin = transform.position + Vector3.left * sortCount / 2 * sortSpace;
        for (int i = 0; i < sortCount; i++)
        {
            randomList.Add(i);
        }

        for(int i = 0; i < sortCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomList.Count);

            GameObject prefab = Instantiate(sortPrefab, origin + Vector3.right * sortSpace * currentSortList.Count, Quaternion.identity);
            prefab.transform.SetParent(this.transform);

            SortElement sortElement = new SortElement(prefab, randomList[randomIndex]);
            sortElement.prefab.transform.localScale = new Vector3(1, sortElement.value, 1);
            //sortElement.prefab.GetComponentInChildren<AudioSource>().pitch = (2f / sortCount) * sortElement.value;

            currentSortList.Add(sortElement);
            randomList.RemoveAt(randomIndex);

        }
    }
    private void ExecuteSort()
    {
        if (isSorting) return;

        if (sortDictionary.TryGetValue(sortType, out Sort currentSort))
        {
            isSorting = true;
            sortName.text = (sortType.ToString() + " Sort");
            StartCoroutine(Execute(currentSort));
        }
    }

    private IEnumerator Execute(Sort currentSort)
    {
        yield return StartCoroutine(currentSort.ExecuteSort(currentSortList, sortSpeed));
        isSorting = false;
    }
}
