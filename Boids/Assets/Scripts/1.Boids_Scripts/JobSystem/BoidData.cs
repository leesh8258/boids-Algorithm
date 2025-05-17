using Unity.Mathematics;

public struct BoidData
{
    public float3 position;
    public float3 forward;
    public float3 egoNormal;
    public float3 obstacleAvoidVec;
    public float additionalSpeed;

}