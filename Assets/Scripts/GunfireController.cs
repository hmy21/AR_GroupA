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
        [Tooltip("Muzzle Flash Prefab")]
        public GameObject muzzlePrefab;
        [Tooltip("Muzzle position object, used to determine the location where flash/bullets are generated")]
        public GameObject muzzlePosition;

        // --- Config ---
        public float shotDelay = 0.5f; // Shooting interval

        // --- Options ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- Projectile (Bullet) ---
        [Tooltip("Bullet prefab, must contain Rigidbody, Collider and BulletController scripts")]
        public GameObject projectilePrefab;
        [Tooltip("When firing bullets, you may need to disable a gun model (such as built-in gun effects)")]
        public GameObject projectileToDisableOnFire;

        // --- Timing ---
        [SerializeField] private float timeLastFired;

        // --- The initial force of the bullet launch ---
        [Header("Bullet Settings")]
        [Tooltip("The initial force applied when the bullet is fired")]
        public float bulletForce = 2000f;

        // --- Crosshair UI ---
        [Header("Crosshair Settings")]
        [Tooltip("Crosshair UI element (RectTransform) to determine the direction of the shot")]
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
            // If you need to perform other logic here, such as scopeActive switching
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }
        }

        /// <summary>
        /// When FullScreenDrag detects a tap on the screen, this method is called to trigger shooting.
        /// </summary>
        public void FireWeapon()
        {
            timeLastFired = Time.time;
            Camera cam = Camera.main;
            Debug.Log("GunfireController.FireWeapon() called");

            // Calculates a ray based on the crosshair to destroy enemies
            ShootFromCrosshair();



            // The following is the original logic of firing bullets, generating muzzle flash, playing sound effects, etc.
            // If you need bullets + flash, keep it; if you only want to destroy the enemy, you can comment out the bullet logic

            // ---------- Bullet logic ----------
            /*rojectilePrefab != null && muzzlePosition != null)
            {
            // Define spawnDistance, which is used to convert the screen coordinates of the crosshair to world coordinates
            float spawnDistance = 0f; // For example, 1 meter
            Vector3 screenPoint = new Vector3(crosshairUI.position.x, crosshairUI.position.y, spawnDistance);
            Vector3 targetPoint = cam.ScreenToWorldPoint(screenPoint);
            // Calculate the firing direction: from the muzzle position to the target point
            Vector3 fireDir = (targetPoint - muzzlePosition.transform.position).normalized;

            // Calculate the bullet generation position: offset a certain distance from the muzzle position along fireDir to avoid overlapping with the muzzle
            float offsetDistance = 0.5f;
            Vector3 spawnPos = muzzlePosition.transform.position + fireDir * offsetDistance;
            // Fix the Y coordinate to be consistent with the muzzle
            spawnPos.y = muzzlePosition.transform.position.y;

            // Generate the base rotation so that the bullet faces fireDir
            Quaternion baseRot = Quaternion.LookRotation(fireDir, Vector3.up);

            // Rotation compensation: If the bullet faces the local X axis by default, we need to adjust it to the positive Z axis
            // Try using -90° or 90°, adjust according to the actual effect
            Quaternion correction = Quaternion.Euler(0, 0, 0); // If it is not suitable, try Quaternion.Euler(0, 90f, 0)
            Quaternion spawnRot = baseRot * correction;

            GameObject bullet = Instantiate(projectilePrefab, spawnPos, spawnRot); 
            bullet.transform.parent = null; // Detach from parent 

            Rigidbody rb = bullet.GetComponent<Rigidbody>(); 
            if (rb != null) 
            { 
            rb.AddForce(fireDir * bulletForce, ForceMode.Impulse); 
            } 

            Debug.Log($"Bullet spawned at: {spawnPos}, fireDir: {fireDir}, bullet.forward: {bullet.transform.forward}"); 
            }*/
            if (muzzlePrefab != null && muzzlePosition != null)
            {
                // Use the same spawnRot as the bullet to generate the firelight, so that the firelight and the bullet are in the same direction
                // Here we directly use the forward direction of muzzlePosition (assuming UpdateGunAim() or other logic has been updated)
                Quaternion muzzleCorrection = Quaternion.Euler(0f, 0f, 0f);
                Quaternion muzzleRot = muzzlePosition.transform.rotation * muzzleCorrection;
                GameObject flash = Instantiate(muzzlePrefab, muzzlePosition.transform.position, muzzleRot);
                flash.transform.SetParent(muzzlePosition.transform);
            }

            // ---------- Optional: Disable an object (such as a built-in gun model ----------
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3f);
            }

            // ---------- Play gunshot----------
            if (source != null && GunShotClip != null)
            {
                // Play sound effects
                source.PlayOneShot(GunShotClip);
            }
        }

        private void ReEnableDisabledProjectile()
        {
            if (projectileToDisableOnFire != null)
                projectileToDisableOnFire.SetActive(true);
        }

        /// <summary>
        /// Use the crosshairs as screen coordinates, ray-detect enemies and destroy them
        /// </summary>
        private void ShootFromCrosshair()
        {
            Debug.Log("ShootFromCrosshair() called in GunfireController");

            Camera cam = Camera.main;
            if (cam == null || crosshairUI == null)
                return;

            // Generates a ray from the crosshair screen coordinates
            Ray ray = cam.ScreenPointToRay(crosshairUI.position);
            Debug.Log("Ray origin: " + ray.origin + ", direction: " + ray.direction);

            // If the ray hits an enemy, it is destroyed
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log("Raycast hit: " + hit.collider.name + " at " + hit.point);
                if (hit.collider.CompareTag("Enemy"))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.takeDamage(10);
                    }
                    //Destroy(hit.collider.gameObject);
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
