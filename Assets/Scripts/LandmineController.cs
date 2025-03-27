using UnityEngine;

public class LandmineController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("��ը��ЧԤ�Ƽ�")]
    public GameObject explosionEffectPrefab;

    [Tooltip("��ը����")]
    public AudioClip explosionSound;

    [Tooltip("��ը����������")]
    public float explosionVolume = 1.0f;

    [Header("Landmine Lifetime")]
    [Tooltip("������δ����ʱ������ʱ�䣨�룩���������Զ�����")]
    public float lifeTime = 30f;

    // ����Ƿ��Ѿ���ը�������ظ�����
    private bool hasExploded = false;

    void Start()
    {
        // ��������� lifeTime ��δ���������Զ�����
        Destroy(gameObject, lifeTime);
    }

    // ȷ������Ԥ�Ƽ��� Collider ��Ϊ Trigger
    void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
            return;

        // �����˽��봥������ʱ��������ը
        if (other.CompareTag("Enemy"))
        {
           
            // ���ٴ������׵ĵ���
            Destroy(other.gameObject);
            Explode();
        }
    }

    /// <summary>
    /// ������ը�����ɱ�ը��Ч�����ű�ը�����������ٵ���
    /// </summary>
    void Explode()
    {
        hasExploded = true;

        // ���ɱ�ը��Ч
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // ���ű�ը����
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }

        // �ӳ����ٵ��ף���ȷ����Ч�������������
        Destroy(gameObject, 0.1f);
    }
}
