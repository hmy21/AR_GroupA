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

    // 标记是否已经爆炸，避免重复触发
    private bool hasExploded = false;

    void Start()
    {
        // 如果地雷在 lifeTime 内未触发，则自动销毁
        Destroy(gameObject, lifeTime);
    }

    // 确保地雷预制件的 Collider 设为 Trigger
    void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
            return;

        // 当敌人进入触发区域时，触发爆炸
        if (other.CompareTag("Enemy"))
        {
           
            // 销毁触碰地雷的敌人
            Destroy(other.gameObject);
            Explode();
        }
    }

    /// <summary>
    /// 触发爆炸：生成爆炸特效、播放爆炸声音，并销毁地雷
    /// </summary>
    void Explode()
    {
        hasExploded = true;

        // 生成爆炸特效
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 播放爆炸声音
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }

        // 延迟销毁地雷，以确保特效和声音播放完毕
        Destroy(gameObject, 0.1f);
    }
}
