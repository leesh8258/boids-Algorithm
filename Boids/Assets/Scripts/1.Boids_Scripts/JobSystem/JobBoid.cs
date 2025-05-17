using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class JobBoid : MonoBehaviour
{
    [Header("Boid Dynamic State")]
    public float3 egoNormalVector = Vector3.zero;
    public float additionalSpeed = 0f;

    private void Start()
    {
        StartCoroutine(SetEgoVector());
    }

    private void Update()
    {
        if (additionalSpeed <= 0f) return;
        additionalSpeed -= Time.deltaTime;
    }

    private IEnumerator SetEgoVector()
    {
        while (true)
        {
            egoNormalVector = UnityEngine.Random.insideUnitSphere;
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        }
    }
}
