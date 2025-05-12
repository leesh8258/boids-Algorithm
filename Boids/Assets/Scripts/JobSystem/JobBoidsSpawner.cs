using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;

public class JobBoidsSpawner : MonoBehaviour
{
    [Header("Spawner Init")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initCount = 100;
    [SerializeField] private float initRadius = 10f;

    [Header("Boid Setting")]
    [SerializeField] private float speed = 5f;

    [Header("Boid Behavior Settings")]
    [SerializeField, Range(0f, 360f)] private float searchFOV = 270f;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float viewRadius = 5f;

    [Header("Boundary Settings")]
    [SerializeField] private float boundaryRadius = 20f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleDetectDistance = 5f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float obstacleWeight = 2f;

    [SerializeField, Range(0f, 5f)] private float separationWeight = 1.5f;
    [SerializeField, Range(0f, 5f)] private float alignmentWeight = 1.0f;
    [SerializeField, Range(0f, 5f)] private float cohesionWeight = 1.0f;
    [SerializeField, Range(0f, 1f)] private float egoWeight = 0.3f;

    private List<GameObject> boids = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < initCount; i++)
        {
            GameObject obj = Instantiate(prefab, UnityEngine.Random.insideUnitSphere * initRadius, UnityEngine.Random.rotation);
            boids.Add(obj);
        }
    }

    private void Update()
    {
        BoidsMove();
    }

    private void BoidsMove()
    {
        int count = boids.Count;
        var boidDataArray = new NativeArray<BoidData>(count, Allocator.TempJob);
        var positionArray = new NativeArray<float3>(count, Allocator.TempJob);
        var directionArray = new NativeArray<float3>(count, Allocator.TempJob);
        var gridMap = new NativeParallelMultiHashMap<int, int>(count, Allocator.TempJob);

        for (int i = 0; i < count; i++)
        {
            var boid = boids[i].GetComponent<JobBoid>();
            float3 pos = boids[i].transform.position;
            float3 forward = boids[i].transform.forward;
            float3 avoidVec = float3.zero;

            if (Physics.Raycast(pos, forward, out RaycastHit hit, obstacleDetectDistance, targetMask))
            {
                avoidVec = hit.normal; // 장애물에서 멀어지는 방향
            }

            boidDataArray[i] = new BoidData
            {
                position = pos,
                forward = forward,
                egoNormal = boid.egoNormalVector,
                obstacleAvoidVec = avoidVec,
                additionalSpeed = boid.additionalSpeed
            };

            int3 cell = BoidGridUtility.WorldToCell(pos, cellSize);
            int hash = BoidGridUtility.Hash(cell);
            gridMap.Add(hash, i);
        }

        float dotThreshold = math.cos(math.radians(searchFOV * 0.5f));

        var job = new BoidMovementJob
        {
            boids = boidDataArray,
            gridMap = gridMap,

            cellSize = cellSize,
            separationRadius = separationRadius,
            viewRadius = viewRadius,
            separationWeight = separationWeight,
            alignmentWeight = alignmentWeight,
            cohesionWeight = cohesionWeight,
            egoWeight = egoWeight,
            boundaryRadius = boundaryRadius,
            obstacleWeight = obstacleWeight,
            speed = speed,
            deltaTime = Time.deltaTime,
            searchDotThreshold = dotThreshold,

            positionsOut = positionArray,
            directionsOut = directionArray
        };

        job.Schedule(count, 64).Complete();

        for (int i = 0; i < count; i++)
        {
            boids[i].transform.position = positionArray[i];
            boids[i].transform.rotation = Quaternion.LookRotation(directionArray[i]);
        }

        boidDataArray.Dispose();
        positionArray.Dispose();
        directionArray.Dispose();
        gridMap.Dispose();
    }
}
