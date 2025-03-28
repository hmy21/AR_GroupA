using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Impact Effect Settings")]
    [Tooltip("碰撞时生成的粒子效果预制件，比如火花或灰尘粒子系统")]
    public GameObject impactEffectPrefab;

    [Tooltip("碰撞时播放的撞击声音")]
    public AudioClip impactSound;

    [Tooltip("撞击声音的音量")]
    public float impactVolume = 1.0f;

    [Header("Bullet Settings")]
    [Tooltip("子弹在未碰撞时的最长存活时间")]
    public float lifeTime = 10f;

    [Tooltip("子弹碰撞后停留的时间（秒），之后自动销毁")]
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
