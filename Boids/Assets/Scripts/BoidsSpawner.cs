using System.Collections.Generic;
using UnityEngine;

public class BoidsSpawner : MonoBehaviour
{
    public static BoidsSpawner Instance;
    
    [Header("Spawner Init")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initCount;
    [SerializeField] private float initRadius;

    [Header("boid Setting")]
    public int boidsMaxCount;
    public float detectRadius;
    public float separationRadius;
    public float speed;

    [Header("Map Setting")]
    [SerializeField] private float maxRadius;

    [Header("Boid Weight Setting")]
    [Range(0f, 10f), SerializeField] private float separationWeight;
    [Range(0f, 10f), SerializeField] private float alignmentWeight;
    [Range(0f, 10f), SerializeField] private float cohensionWeight;
    [Range(0f, 1f), SerializeField] private float egoWeight;
    [Range(1f, 10f), SerializeField] private float boundaryWeight;

    private ICalculate Separation, Alignment, Cohension;
    public List<GameObject> boids = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        Separation = new Separation();
        Alignment = new Alignment();
        Cohension = new Cohension();

        for(int i = 0; i < initCount; i++)
        {
            boids.Add(Instantiate(prefab, Random.insideUnitSphere * initRadius, Random.rotation));
        }
    }

    private void Update()
    {
        BoidsMove();
    }

    private void BoidsMove()
    {
        foreach (GameObject boid in boids)
        {
            Boid boidComponent = boid.GetComponent<Boid>();
            Vector3 dir;

            dir = Alignment.Calculate(boid.transform, boidComponent.detectList) * alignmentWeight;
            dir += Cohension.Calculate(boid.transform, boidComponent.detectList) * cohensionWeight;
            dir += Separation.Calculate(boid.transform, boidComponent.separationList) * separationWeight;

            if(maxRadius < boid.transform.position.magnitude)
            {
                Vector3 offset = boid.transform.position - Vector3.zero;
                dir += offset.normalized * (maxRadius - offset.magnitude) * boundaryWeight;
            }

            dir += boidComponent.egoNormalVector * egoWeight;
            //Àå¾Ö¹°


            dir = Vector3.Lerp(boid.transform.forward, dir, Time.deltaTime);
            dir.Normalize();

            boid.transform.position += dir * speed * Time.deltaTime;
            boid.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

}
