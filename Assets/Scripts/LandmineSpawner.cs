using System.Collections;
using UnityEngine;


public class LandmineSpawner : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("��ը��ЧԤ�Ƽ�")]
    public GameObject explosionEffectPrefab;
    [Tooltip("��ը����")]
    public AudioClip explosionSound;
    [Tooltip("��ը��������")]
    public float explosionVolume = 1.0f;

    [Header("Landmine Settings")]
    [Tooltip("������δ����ʱ������ʱ�䣨�룩���������Զ�����")]
    public float lifeTime = 30f;

    // ����Ƿ��Ѿ���ը�������ظ�����
    private bool hasExploded = false;

    void Start()
    {
        // ������ lifeTime ��δ�������Զ�����
        Destroy(gameObject, lifeTime);
    }

    // Ϊ�˷����⣬���׵� Collider ��Ϊ Trigger
    void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
            return;

        // ������봥��������ǵ��ˣ��򴥷���ը
        if (other.CompareTag("Enemy"))
        {
            // ���ٴ������׵ĵ���
            Destroy(other.gameObject);

            // ��ը�����ٵ�������
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


        // �ӳ����ٵ��ף������ӳٺ����٣���ȷ����Ч���ţ�
        Destroy(gameObject, 0.1f);
    }
}
