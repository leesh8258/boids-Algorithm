using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct BoidMovementJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<BoidData> boids;
    [ReadOnly] public NativeParallelMultiHashMap<int, int> gridMap;

    [ReadOnly] public float cellSize;
    [ReadOnly] public float separationRadius;
    [ReadOnly] public float viewRadius;
    [ReadOnly] public float separationWeight;
    [ReadOnly] public float alignmentWeight;
    [ReadOnly] public float cohesionWeight;
    [ReadOnly] public float egoWeight;
    [ReadOnly] public float boundaryRadius;
    [ReadOnly] public float boundaryWeight;
    [ReadOnly] public float obstacleWeight;
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float searchDotThreshold;
    [ReadOnly] public float speed;

    public NativeArray<float3> positionsOut;
    public NativeArray<float3> directionsOut;

    public void Execute(int index)
    {
        BoidData self = boids[index];
        int3 selfCell = BoidGridUtility.WorldToCell(self.position, cellSize);

        float3 separation = float3.zero;
        float3 alignment = float3.zero;
        float3 cohesion = float3.zero;
        float3 boundaryVec = float3.zero;
        int neighborCount = 0;
        int separationCount = 0;

        // Grid ±â¹Ý ÀÌ¿ô Å½»ö
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
                for (int dz = -1; dz <= 1; dz++)
                {
                    int3 offset = new int3(dx, dy, dz);
                    float3 offsetDir = math.normalize(math.float3(offset));

                    if (math.dot(offsetDir, self.forward) < searchDotThreshold)
                        continue;

                    int3 neighborCell = selfCell + offset;
                    int hash = BoidGridUtility.Hash(neighborCell);

                    if (gridMap.TryGetFirstValue(hash, out int neighborIndex, out var it))
                    {
                        do
                        {
                            if (neighborIndex == index) continue;

                            BoidData neighbor = boids[neighborIndex];
                            float dist = math.distance(self.position, neighbor.position);
                            if (dist > viewRadius) continue;

                            alignment += neighbor.forward;
                            cohesion += neighbor.position;
                            neighborCount++;

                            if (dist < separationRadius)
                            {
                                separation += self.position - neighbor.position;
                                separationCount++;
                            }

                        } while (gridMap.TryGetNextValue(out neighborIndex, ref it));
                    }
                }

        if (neighborCount > 0)
        {
            alignment = math.normalize(alignment / neighborCount);
            cohesion = math.normalize((cohesion / neighborCount) - self.position);
        }

        if (separationCount > 0)
        {
            separation = math.normalize(separation / separationCount);
        }

        float3 dir = self.forward;
        dir += alignment * alignmentWeight;
        dir += cohesion * cohesionWeight;
        dir += separation * separationWeight;
        dir += self.egoNormal * egoWeight;
        dir += self.obstacleAvoidVec * obstacleWeight;
        dir = math.normalize(math.lerp(self.forward, dir, deltaTime));

        float3 newPos = self.position + dir * (speed + self.additionalSpeed) * deltaTime;
        newPos = WrapPosition(newPos);
        positionsOut[index] = newPos;
        directionsOut[index] = dir;
    }

    private float3 WrapPosition(float3 pos)
    {
        if (pos.x > boundaryRadius) pos.x = -boundaryRadius;
        else if (pos.x < -boundaryRadius) pos.x = boundaryRadius;

        if (pos.y > boundaryRadius) pos.y = -boundaryRadius;
        else if (pos.y < -boundaryRadius) pos.y = boundaryRadius;

        if (pos.z > boundaryRadius) pos.z = -boundaryRadius;
        else if (pos.z < -boundaryRadius) pos.z = boundaryRadius;

        return pos;
    }
}
