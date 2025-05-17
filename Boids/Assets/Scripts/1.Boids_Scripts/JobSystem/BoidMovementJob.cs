using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct BoidMovementJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<BoidData> boids;
    [ReadOnly] public NativeParallelMultiHashMap<int, int> gridMap;

    [ReadOnly] public float cellSize; //Physics 대체 (boids 그룹)
    [ReadOnly] public float separationRadius; //Physics 대체 (boids 그룹 중 separation)
    [ReadOnly] public float viewRadius; // 시야 거리

    //가중치
    [ReadOnly] public float separationWeight;
    [ReadOnly] public float alignmentWeight;
    [ReadOnly] public float cohesionWeight;
    [ReadOnly] public float egoWeight;
    [ReadOnly] public float boundaryWeight;
    [ReadOnly] public float obstacleWeight;

    [ReadOnly] public float boundaryRadius; //행동 거리 제한
    [ReadOnly] public float deltaTime; //시간
    [ReadOnly] public float searchDotThreshold; //시야각
    [ReadOnly] public float speed; //속도

    public NativeArray<float3> positionsOut; //transform position 배열
    public NativeArray<float3> directionsOut; //각 방향벡터 배열

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

        // Grid 기반 이웃 탐색
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

        if(neighborCount == 0)
        {
            alignment = self.forward;
        }

        float boidMagnitude;
        boidMagnitude = math.sqrt(
            self.position.x * self.position.x +
            self.position.y * self.position.y +
            self.position.z * self.position.z
            );

        float3 dir;
        dir = alignment * alignmentWeight;
        dir += cohesion * cohesionWeight;
        dir += separation * separationWeight;
        dir += self.obstacleAvoidVec * obstacleWeight;
        dir += self.egoNormal * egoWeight;

        if(boundaryRadius < boidMagnitude)
        {
            float3 offset = self.position - float3.zero;
            float offsetMagnitude = math.sqrt(
                offset.x * offset.x +
                offset.y * offset.y +
                offset.z * offset.z
                );
            dir += math.normalize(offset) * (boundaryRadius - offsetMagnitude) * boundaryWeight;
        }

        dir = math.normalize(math.lerp(self.forward, dir, deltaTime));

        float3 newPos = self.position + dir * (speed + self.additionalSpeed) * deltaTime;
        positionsOut[index] = newPos;
        directionsOut[index] = dir;
    }


}
