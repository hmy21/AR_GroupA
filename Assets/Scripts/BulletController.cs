using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Impact Effect Settings")]
    [Tooltip("��ײʱ���ɵ�����Ч��Ԥ�Ƽ�������𻨻�ҳ�����ϵͳ")]
    public GameObject impactEffectPrefab;

    [Tooltip("��ײʱ���ŵ�ײ������")]
    public AudioClip impactSound;

    [Tooltip("ײ������������")]
    public float impactVolume = 1.0f;

    [Header("Bullet Settings")]
    [Tooltip("�ӵ���δ��ײʱ������ʱ��")]
    public float lifeTime = 10f;

    [Tooltip("�ӵ���ײ��ͣ����ʱ�䣨�룩��֮���Զ�����")]
    public float landingDuration = 2f;

    // ���ڱ���ӵ��Ƿ��Ѿ�������ײ
    private bool hasLanded = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ����ӵ��� lifeTime ��û����ײ�����Զ�����
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // ����Ѿ��������ײ���򷵻�
        if (hasLanded)
            return;

        // ��ȡ��ײ�Ӵ���
        ContactPoint contact = collision.contacts[0];

        // ����ײ����Ч
        if (impactEffectPrefab != null)
        {
            // ʹ��Ч������ײ�淨��
            Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        }

        // ����ײ����Ч
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, contact.point, impactVolume);
        }

        // ���ӵ���Ȼ���£�
      
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // ������ק��ģ�����Ħ����ʹ�ӵ���Ȼ����
            rb.drag = 5f;
            rb.angularDrag = 5f;
            // ���� Use Gravity Ϊ true�����ӵ������ܵ�����Ӱ��
        }

        hasLanded = true;
        // ����Э�̣��� landingDuration ��������ӵ�
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(landingDuration);
        Destroy(gameObject);
    }
}
