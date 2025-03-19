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
        [Tooltip("枪口位置对象，用于确定发射子弹的位置")]
        public GameObject muzzlePrefab;
        public GameObject muzzlePosition;

        // --- Config ---
        public float shotDelay = 0.5f; // 射击间隔

        // --- Options ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- Projectile (Bullet) ---
        [Tooltip("子弹预制件，必须包含 Rigidbody、Collider 和 BulletController 脚本")]
        public GameObject projectilePrefab;
        [Tooltip("当发射子弹时，可能需要禁用某个枪械模型（例如内置枪械效果）")]
        public GameObject projectileToDisableOnFire;

        // --- Timing ---
        [SerializeField] private float timeLastFired;

        // --- 子弹发射的初始力度 ---
        [Header("Bullet Settings")]
        [Tooltip("子弹发射时施加的初始力")]
        public float bulletForce = 2000f;

        // --- Crosshair UI ---
        [Header("Crosshair Settings")]
        [Tooltip("准星 UI 元素 (RectTransform)，用于确定射击方向")]
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
            // 每帧更新枪口朝向：根据当前准星位置计算射击方向
            UpdateGunAim();

            // 处理触摸输入：轻点屏幕触发开火（拖拽由 FullScreenDrag 脚本管理）
            HandleTouchInput();

            // Toggle scope 状态
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// 根据准星 UI 的位置更新枪口发射方向
        /// </summary>
        private void UpdateGunAim()
        {
            if (Camera.main != null && crosshairUI != null && muzzlePosition != null)
            {
                // 获取准星的屏幕坐标（世界坐标不适用，因为它是在 Canvas 中）
                Vector3 screenPoint = crosshairUI.position;
                // 从摄像机中生成一条射线，射线起点为摄像机位置，方向为从屏幕点延伸出去
                Ray ray = Camera.main.ScreenPointToRay(screenPoint);
                // 取射线在一定距离（例如 10 米处）的一个点作为目标
                float targetDistance = 10f;
                Vector3 targetPoint = ray.GetPoint(targetDistance);
                // 计算从枪口位置到目标点的方向
                Vector3 newDir = (targetPoint - muzzlePosition.transform.position).normalized;
                // 更新枪口的 forward，使得子弹会沿着这个方向发射
                muzzlePosition.transform.forward = newDir;
            }
        }

        /// <summary>
        /// 处理触摸输入：在轻点屏幕时触发开火
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
                // 当触摸结束且移动距离较小（轻点）时触发开火
                if (touch.phase == TouchPhase.Ended && touch.deltaPosition.magnitude < 10f)
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
            Camera cam = Camera.main;
            // 生成子弹（添加位置偏移和旋转补偿）
            if (projectilePrefab != null && muzzlePosition != null)
            {
                float offsetDistance = 0.2f; // 根据需要调整偏移距离
                                             // 计算子弹生成位置 = 枪口位置 + 枪口 forward * offsetDistance
                Vector3 spawnPos = muzzlePosition.transform.position + muzzlePosition.transform.forward * offsetDistance;
                // 固定 spawnPos 的 Y 坐标与枪口一致（防止上下偏移）
                spawnPos.y = muzzlePosition.transform.position.y;

                // 初始旋转直接使用枪口旋转
                Quaternion muzzleCorrection = Quaternion.Euler(0f, -90f, 0f);
                Quaternion bulletRot = muzzlePosition.transform.rotation * muzzleCorrection;
                GameObject bullet = Instantiate(projectilePrefab, muzzlePosition.transform.position, bulletRot);

                bullet.transform.parent = null; // 脱离父级，确保独立物理控制

              
                // 这样模型的 up 将转为 forward，使得子弹视觉上与物理发射方向一致
                bullet.transform.rotation = Quaternion.Euler(90f, 0f, 0f) * bullet.transform.rotation;

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // 使用枪口的 forward 方向施加力（即正 Z 轴方向）
                    rb.AddForce(muzzlePosition.transform.forward * bulletForce, ForceMode.Impulse);
                }

                Debug.Log($"Bullet instantiated at: {bullet.transform.position}, Bullet forward: {bullet.transform.forward}");
            }

            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3f);
            }

            // ---------- 枪口火光 ----------
            // 如果火光预制件存在，生成火光并进行旋转补偿
            if (muzzlePrefab != null && muzzlePosition != null)
            {
                // 如果火光预制件默认朝向为向右（local forward 为 X 轴），我们需要绕 Y 轴旋转 -90°，使其朝向枪口的 forward（正 Z 轴）
                Quaternion muzzleCorrection = Quaternion.Euler(0f, -90f, 0f);
                Quaternion muzzleRot = muzzlePosition.transform.rotation * muzzleCorrection;
                Instantiate(muzzlePrefab, muzzlePosition.transform.position, muzzleRot);
            }

            // ---------- 可选：禁用某个对象（如内置枪械显示效果） ----------
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3f);
            }

            // ---------- 播放枪声 ----------
            if (source != null)
            {
                if (source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    AudioSource newAS = Instantiate(source);
                    if ((newAS = Instantiate(source)) != null &&
                        newAS.outputAudioMixerGroup != null &&
                        newAS.outputAudioMixerGroup.audioMixer != null)
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
