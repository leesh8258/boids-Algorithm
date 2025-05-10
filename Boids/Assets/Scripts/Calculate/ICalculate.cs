using System.Collections.Generic;
using UnityEngine;

public interface ICalculate
{
    Vector3 Calculate(Transform self, List<Transform> neighbor);
}