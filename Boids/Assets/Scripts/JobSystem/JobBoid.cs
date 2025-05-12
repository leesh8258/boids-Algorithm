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

    private System.Collections.IEnumerator SetEgoVector()
    {
        while (true)
        {
            // 방향 변화 감지를 통한 egoVector 설정
            egoNormalVector = UnityEngine.Random.insideUnitSphere; // 임시값 (원하는 로직으로 대체 가능)
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 8f);
    }
}
