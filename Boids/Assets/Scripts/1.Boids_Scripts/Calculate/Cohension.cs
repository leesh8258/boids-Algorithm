using System.Collections.Generic;
using UnityEngine;

public class Cohension : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbors)
    {
        if(self == null || neighbors == null || neighbors.Count == 0) return Vector3.zero;

        Vector3 avgPosition = Vector3.zero;

        foreach(var neighbor in neighbors)
        {
            avgPosition += neighbor.position;
        }

        avgPosition /= neighbors.Count;
        return (avgPosition - self.position).normalized;
    }
}
