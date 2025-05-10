using System.Collections.Generic;
using UnityEngine;

public class Alignment : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbor)
    {
        if(self == null) return Vector3.zero;
        if (neighbor.Count == 0 || neighbor == null) return self.forward;

        Vector3 direction = Vector3.zero;

        foreach(Transform neighborTransform in neighbor)
        {
            direction += neighborTransform.forward;
        }

        direction /= neighbor.Count;
        return direction.normalized;
    }
}
