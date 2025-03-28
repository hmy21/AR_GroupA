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

    
    private bool hasExploded = false;

    void Start()
    {
        
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
            return;

        
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) 
            {
                enemy.takeDamage(10);
            }
            
         
            Explode();
        }
    }


    void Explode()
    {
        hasExploded = true;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }

        Destroy(gameObject, 0.1f);
    }
}
