using UnityEngine;

public class LandmineController : MonoBehaviour
{
    [Header("Explosion Settings")]
    [Tooltip("爆炸特效预制件")]
    public GameObject explosionEffectPrefab;

    [Tooltip("爆炸声音")]
    public AudioClip explosionSound;

    [Tooltip("爆炸声音的音量")]
    public float explosionVolume = 1.0f;

    [Header("Landmine Lifetime")]
    [Tooltip("地雷在未触发时的最长存活时间（秒），超过后自动销毁")]
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
