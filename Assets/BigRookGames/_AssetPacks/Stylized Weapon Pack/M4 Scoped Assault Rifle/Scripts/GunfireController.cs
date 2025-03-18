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
        [Tooltip("枪口位置对象，用于确定发射子弹的位置和初始方向")]
        public GameObject muzzlePosition;

        // --- Config ---
        public float shotDelay = .5f;   // 射击间隔
        public bool rotate = false;     // 固定枪械（不自动旋转）
        public float rotationSpeed = .25f;

        // --- Options ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- Projectile (Bullet) ---
        [Tooltip("子弹预制件，必须包含 Rigidbody、Collider 和 BulletController 脚本")]
        public GameObject projectilePrefab;
        [Tooltip("当发射子弹时，可能需要禁用某个枪械模型，如显示隐藏内置枪械效果")]
        public GameObject projectileToDisableOnFire;

        // --- Timing ---
        [SerializeField] private float timeLastFired;

        // --- Bullet Force ---
        [Header("Bullet Settings")]
        [Tooltip("子弹发射时施加的初始力")]
        public float bulletForce = 2000f;

        // --- Crosshair UI ---
        [Header("Crosshair Settings")]
        [Tooltip("准星 UI 元素（RectTransform），允许拖拽改变其位置")]
        public RectTransform crosshairUI;

        private void Start()
        {
            if (source != null)
                source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;
        }

        private void Update()
        {
            // 根据准星位置更新枪械瞄准方向
            UpdateGunAim();

            // 处理触摸输入（区分拖拽和轻点）
            HandleTouchInput();

            // Toggle scope 状态（保持不变）
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// 根据当前准星 UI 位置更新枪械（或枪口）的朝向
        /// </summary>
        private void UpdateGunAim()
        {
            if (Camera.main != null && crosshairUI != null && muzzlePosition != null)
            {
                // 获取准星的屏幕坐标
                Vector3 screenPoint = crosshairUI.position;
                // 将屏幕坐标转换为世界射线
                Ray ray = Camera.main.ScreenPointToRay(screenPoint);
                // 在射线上的某个距离处计算目标点
                Vector3 targetPoint = ray.GetPoint(1.0f); // 1米距离，可自行调整
                Vector3 newDir = (targetPoint - muzzlePosition.transform.position).normalized;

                // 更新枪口朝向
                muzzlePosition.transform.forward = newDir;
                // 如果需要让整个枪械也朝向同方向
                transform.rotation = Quaternion.LookRotation(newDir, Vector3.up);
            }
        }

        /// <summary>
        /// 处理触摸输入：区分拖拽和轻点。拖拽由 CrosshairDrag 脚本处理，
        /// 这里检测轻点（触摸结束且移动量小）触发射击。
        /// </summary>
        private void HandleTouchInput()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;
                ShootFromMuzzle();
            }
#endif
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                // 当触摸结束时且移动量较小时触发开火
                if (touch.phase == TouchPhase.Ended && touch.deltaPosition.magnitude < 5f)
                {
                    ShootFromMuzzle();
                }
            }
        }

        /// <summary>
        /// 从枪口位置发射子弹
        /// </summary>
        private void ShootFromMuzzle()
        {
            FireWeapon();
        }

        /// <summary>
        /// 发射子弹：在枪口位置生成子弹，并施加初始力
        /// </summary>
        public void FireWeapon()
        {
            timeLastFired = Time.time;

            // 生成枪口火光效果（如果需要）
            // 如果你有枪口火光预制件，可在此处实例化

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

            // 播放枪声
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

        private void ReEnableDisabledProjectile()
        {
            if (projectileToDisableOnFire != null)
                projectileToDisableOnFire.SetActive(true);
        }
    }
}
