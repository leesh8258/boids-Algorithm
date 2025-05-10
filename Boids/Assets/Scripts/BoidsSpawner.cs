using System.Collections.Generic;
using UnityEngine;

public class BoidsSpawner : MonoBehaviour
{
    public static BoidsSpawner Instance;

    [Header("Spawner Init")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int InitCount;
    [SerializeField] private float InitRadius;

    private List<GameObject> boids = new List<GameObject>();
    public List<GameObject> Boids => boids;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        for(int i = 0; i <InitCount; i++)
        {
            boids.Add(Instantiate(prefab, Random.insideUnitCircle * InitRadius, Random.rotation));
        }
    }
}
