using Unity.Mathematics;

public static class BoidGridUtility
{
    public static int3 WorldToCell(float3 position, float cellSize)
    {
        return new int3(
            (int)math.floor(position.x / cellSize),
            (int)math.floor(position.y / cellSize),
            (int)math.floor(position.z / cellSize));
    }

    public static int Hash(int3 cell)
    {
        return cell.x * 73856093 ^ cell.y * 19349663 ^ cell.z * 83492791;
    }
}
