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
            // ���� ��ȭ ������ ���� egoVector ����
            egoNormalVector = UnityEngine.Random.insideUnitSphere; // �ӽð� (���ϴ� �������� ��ü ����)
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 8f);
    }
}
