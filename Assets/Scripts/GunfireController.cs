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
        [Tooltip("枪口火光预制件")]
        public GameObject muzzlePrefab;
        [Tooltip("枪口位置对象，用于确定火光/子弹的生成位置")]
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
            // 如果你需要在此进行其他逻辑，如 scopeActive 切换
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// 当 FullScreenDrag 检测到轻点屏幕时，会调用此方法触发射击
        /// </summary>
        public void FireWeapon()
        {
            timeLastFired = Time.time;
            Camera cam = Camera.main;
            Debug.Log("GunfireController.FireWeapon() called");

            // 计算基于准星的射线，用于销毁敌人
            ShootFromCrosshair();



            // 以下是原先发射子弹、生成枪口火光、播放音效等逻辑
            // 若你需要子弹+火光，就保留；若你只想销毁敌人，可以注释掉子弹逻辑

            // ---------- 子弹逻辑 ----------
           /*rojectilePrefab != null && muzzlePosition != null)
            {
                // 定义 spawnDistance，用于将准星的屏幕坐标转换为世界坐标
                float spawnDistance = 0f; // 例如 1 米
                Vector3 screenPoint = new Vector3(crosshairUI.position.x, crosshairUI.position.y, spawnDistance);
                Vector3 targetPoint = cam.ScreenToWorldPoint(screenPoint);
                // 计算发射方向：从枪口位置指向目标点
                Vector3 fireDir = (targetPoint - muzzlePosition.transform.position).normalized;

                // 计算子弹生成位置：从枪口位置沿 fireDir 偏移一定距离，避免与枪口重叠
                float offsetDistance = 0.5f;
                Vector3 spawnPos = muzzlePosition.transform.position + fireDir * offsetDistance;
                // 固定 Y 坐标与枪口一致
                spawnPos.y = muzzlePosition.transform.position.y;

                // 生成基础旋转，使子弹朝向 fireDir
                Quaternion baseRot = Quaternion.LookRotation(fireDir, Vector3.up);

                // 旋转补偿：如果子弹默认朝向是局部 X 轴，我们需要将其调整到正 Z 轴
                // 尝试使用 -90° 或 90°，根据实际效果调整
                Quaternion correction = Quaternion.Euler(0, 0, 0);  // 若不合适，可试 Quaternion.Euler(0, 90f, 0)
                Quaternion spawnRot = baseRot * correction;

                GameObject bullet = Instantiate(projectilePrefab, spawnPos, spawnRot);
                bullet.transform.parent = null; // 脱离父级

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(fireDir * bulletForce, ForceMode.Impulse);
                }

                Debug.Log($"Bullet spawned at: {spawnPos}, fireDir: {fireDir}, bullet.forward: {bullet.transform.forward}");
            }*/

            if (muzzlePrefab != null && muzzlePosition != null)
            {
                // 使用与子弹相同的 spawnRot 生成火光，使火光和子弹方向一致
                // 这里直接使用 muzzlePosition 的 forward 方向（假设 UpdateGunAim() 或其他逻辑已更新）
                Quaternion muzzleCorrection = Quaternion.Euler(0f, 0f, 0f);
                Quaternion muzzleRot = muzzlePosition.transform.rotation * muzzleCorrection;
                GameObject flash = Instantiate(muzzlePrefab, muzzlePosition.transform.position, muzzleRot);
                flash.transform.SetParent(muzzlePosition.transform);
            }

            // ---------- 可选：禁用某个对象（如内置枪械模型） ----------
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3f);
            }

            // ---------- 播放枪声 ----------
            if (source != null && GunShotClip != null)
            {
                // 播放音效
                source.PlayOneShot(GunShotClip);
            }
        }

        private void ReEnableDisabledProjectile()
        {
            if (projectileToDisableOnFire != null)
                projectileToDisableOnFire.SetActive(true);
        }

        /// <summary>
        /// 以准星为屏幕坐标，射线检测敌人并销毁
        /// </summary>
        private void ShootFromCrosshair()
        {
            Debug.Log("ShootFromCrosshair() called in GunfireController");

            Camera cam = Camera.main;
            if (cam == null || crosshairUI == null)
                return;

            // 从准星屏幕坐标生成射线
            Ray ray = cam.ScreenPointToRay(crosshairUI.position);
            Debug.Log("Ray origin: " + ray.origin + ", direction: " + ray.direction);

            // 若射线击中敌人，则销毁
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log("Raycast hit: " + hit.collider.name + " at " + hit.point);
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Enemy destroyed at " + hit.point);
                }
                else
                {
                    Debug.Log("Hit object tag: " + hit.collider.tag);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }
    }
}
