using System.Collections.Generic;
using UnityEngine;

public class Obstacle : ICalculate
{
    public Vector3 Calculate(Transform self, List<Transform> neighbors)
    {
        RaycastHit hit;
        Vector3 obstacleVec = Vector3.zero;
        Boid boid = self.GetComponent<Boid>();

        if(Physics.Raycast(self.position, self.forward, out hit, BoidsSpawner.Instance.dodgeObstacleDistance, BoidsSpawner.Instance.targetMask))
        {
            Debug.DrawLine(self.position, hit.point, Color.black);
            obstacleVec = hit.normal;
            boid.additionalSpeed = BoidsSpawner.Instance.dodgeSpeed;
        }

        return obstacleVec;
    }
}
