using UnityEngine;
using UnityEngine.EventSystems;

namespace BigRookGames.Weapons
{
    public class GunfireController : MonoBehaviour
    {
        // --- Audio ---
        public AudioClip GunShotClip;
        public AudioSource source;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

        // --- Muzzle ---
        public GameObject muzzlePrefab;
        public GameObject muzzlePosition;

        // --- Config ---
        public bool autoFire;         // 自动开火（如果你不需要自动开火，可保持关闭）
        public float shotDelay = .5f;   // 射击间隔
        public bool rotate = false;     // 如果枪械固定在屏幕中心，不需要旋转
        public float rotationSpeed = .25f;

        // --- Options ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- Projectile (Bullet) ---
        [Tooltip("子弹预制件，必须包含 Rigidbody、Collider 和 BulletController 脚本")]
        public GameObject projectilePrefab;
        [Tooltip("当发射子弹时，可能需要禁用某个枪械模型，例如显示隐藏内置的枪械效果")]
        public GameObject projectileToDisableOnFire;

        // --- Timing ---
        [SerializeField] private float timeLastFired;

        // --- 子弹发射的初始力度 ---
        [Header("Bullet Settings")]
        [Tooltip("子弹发射时施加的初始力")]
        public float bulletForce = 2000f;

        private void Start()
        {
            if (source != null)
                source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;
        }

        private void Update()
        {
            // 如果需要自动旋转枪械，可设置 rotate 为 true，这里固定在屏幕中间,不旋转
            if (rotate)
            {
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y + rotationSpeed,
                    transform.localEulerAngles.z
                );
            }

            // 处理触摸输入：点击屏幕触发射击
            HandleTouchInput();

            // Toggle scope 状态
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// 处理触摸输入，检测用户点击时从屏幕中心发射子弹
        /// </summary>
        private void HandleTouchInput()
        {
            // Editor 下用鼠标测试（可选）
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                // 可添加 UI 排除判断
                ShootFromMuzzle();
            }
#endif

            // 真机下处理触摸
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    // 如果点击到 UI，则忽略
                    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                        return;
                    ShootFromMuzzle();
                }
            }
        }

        /// <summary>
        /// 射击逻辑：在枪口位置发射子弹，并播放开火音效和火口特效
        /// </summary>
        public void FireWeapon()
        {
            // 记录射击时间
            timeLastFired = Time.time;

            // --- 生成枪口火光效果 ---
            if (muzzlePrefab != null && muzzlePosition != null)
            {
                Instantiate(muzzlePrefab, muzzlePosition.transform.position, muzzlePosition.transform.rotation);
            }

            // 生成子弹（添加位置偏移和旋转补偿）
            if (projectilePrefab != null && muzzlePosition != null)
            {
                float offsetDistance = -0.3f; // 根据需求调整
                                             // 计算水平方向上的 fireDirection（忽略所有垂直分量）
                Vector3 fireDirection = Vector3.ProjectOnPlane(muzzlePosition.transform.forward, Vector3.up).normalized;
                // 计算生成位置
                Vector3 spawnPos = muzzlePosition.transform.position + fireDirection * offsetDistance;
                // 强制 spawnPos 的 Y 坐标与枪口保持一致
                spawnPos.y = muzzlePosition.transform.position.y;

                Quaternion correction = Quaternion.Euler(-90f, 0, -90f); // -90°绕Y轴
                Quaternion spawnRot = muzzlePosition.transform.rotation * correction;


                GameObject bullet = Instantiate(projectilePrefab, spawnPos, spawnRot);
                bullet.transform.parent = null;

     
            }

            // --- 可选：禁用某个对象 ---
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3);
            }

            // --- 播放开火音效 ---
            if (source != null)
            {
                if (source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    AudioSource newAS = Instantiate(source);
                    if ((newAS = Instantiate(source)) != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
                        newAS.pitch = Random.Range(audioPitch.x, audioPitch.y);
                        newAS.PlayOneShot(GunShotClip);
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }
        }

        /// <summary>
        /// 射击入口：直接调用 FireWeapon()。如果你希望枪械始终固定在屏幕中心，
        /// 请确保枪械对象已通过 Canvas 或固定在摄像机上实现固定显示效果。
        /// </summary>
        private void ShootFromMuzzle()
        {
            // 这里也可以添加射击间隔判断（例如 if (Time.time >= timeLastFired + shotDelay)）
            FireWeapon();
        }

        private void ReEnableDisabledProjectile()
        {
            projectileToDisableOnFire.SetActive(true);
        }
    }
}
