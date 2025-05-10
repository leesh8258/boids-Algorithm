using System.Collections.Generic;
using UnityEngine;

public class Separation : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbor)
    {
        if(self == null || neighbor == null) return Vector3.zero;

        Vector3 direction = Vector3.zero;

        foreach(Transform neighborTransform in neighbor)
        {
            direction += self.position - neighborTransform.position;
        }

        return direction.normalized;
    }
}
