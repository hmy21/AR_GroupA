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

   
    private bool hasLanded = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
    
        if (hasLanded)
            return;

       
        ContactPoint contact = collision.contacts[0];

        
        if (impactEffectPrefab != null)
        {
            
            Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        }

     
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, contact.point, impactVolume);
        }

  
      
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.drag = 5f;
            rb.angularDrag = 5f;
        
        }

        hasLanded = true;
  
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(landingDuration);
        Destroy(gameObject);
    }
}
