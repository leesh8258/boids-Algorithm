using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private struct SortStrcut
    {
        public SortType type;
        public Sort sort;
    }

    [SerializeField] private List<SortStrcut> sortStructs;
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
        foreach(SortStrcut component in sortStructs)
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
        //그 리스트를 넘겨주는 방식
        //ExcuteSort();
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
        for (int i = 0; i < sortCount; i++)
        {
            randomList.Add(i);
        }

        for(int i = 0; i < sortCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomList.Count);

            GameObject prefab = Instantiate(sortPrefab, Vector3.left * sortCount / 2 * sortSpace + Vector3.right * sortSpace * currentSortList.Count, Quaternion.identity);
            SortElement sortElement = new SortElement(prefab, randomList[randomIndex]);
            sortElement.prefab.transform.localScale = new Vector3(1, sortElement.value, 1);
            //sortElement.prefab.GetComponentInChildren<AudioSource>().pitch = (2f / sortCount) * sortElement.value;
            currentSortList.Add(sortElement);
            randomList.RemoveAt(randomIndex);

        }
    }
    private void ExcuteSort()
    {
        if (isSorting) return;

        if (sortDictionary.TryGetValue(sortType, out Sort currentSort))
        {
            isSorting = true;
            StartCoroutine(Execute(currentSort));
        }
    }

    //여따가 currentList를 넘겨줘야함.
    private IEnumerator Execute(Sort currentSort)
    {
        currentSort.ExecuteSort();
        yield return new WaitUntil(() => currentSort.isSorting);
        isSorting = false;
    }
}
