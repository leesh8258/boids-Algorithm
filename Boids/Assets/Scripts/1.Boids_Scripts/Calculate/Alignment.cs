using System.Collections.Generic;
using UnityEngine;

public class Alignment : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbors)
    {
        if(self == null) return Vector3.zero;
        if (neighbors.Count == 0 || neighbors == null) return self.forward;

        Vector3 direction = Vector3.zero;

        foreach(var neighbor in neighbors)
        {
            direction += neighbor.forward;
        }

        direction /= neighbors.Count;
        return direction.normalized;
    }
}
