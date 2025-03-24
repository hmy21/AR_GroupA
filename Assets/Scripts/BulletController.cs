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

    // 用于标记子弹是否已经发生碰撞
    private bool hasLanded = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 如果子弹在 lifeTime 内没有碰撞，则自动销毁
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 如果已经处理过碰撞，则返回
        if (hasLanded)
            return;

        // 获取碰撞接触点
        ContactPoint contact = collision.contacts[0];

        // 生成撞击特效
        if (impactEffectPrefab != null)
        {
            // 使特效朝向碰撞面法线
            Instantiate(impactEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal));
        }

        // 播放撞击音效
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, contact.point, impactVolume);
        }

        // 让子弹自然落下：
      
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // 增加拖拽，模拟地面摩擦，使子弹自然沉降
            rb.drag = 5f;
            rb.angularDrag = 5f;
            // 保持 Use Gravity 为 true，让子弹继续受到重力影响
        }

        hasLanded = true;
        // 启动协程，在 landingDuration 秒后销毁子弹
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(landingDuration);
        Destroy(gameObject);
    }
}
