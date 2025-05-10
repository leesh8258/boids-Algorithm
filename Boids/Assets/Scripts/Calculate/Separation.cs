using System.Collections.Generic;
using UnityEngine;

public class Separation : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbors)
    {
        if(self == null || neighbors == null) return Vector3.zero;

        Vector3 direction = Vector3.zero;

        foreach(var neighbor in neighbors)
        {
            direction += self.position - neighbor.position;
        }

        return direction.normalized;
    }
}
