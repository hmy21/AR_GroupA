using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARGameController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARPlaneManager planeManager;       // 检测平面
    [SerializeField] private ARCameraManager cameraManager;     // 获取光照估计
    [SerializeField] private ARRaycastManager raycastManager;   // 若需要触摸点检测平面，可用它

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;            // 敌人预制件

    // 存储生成的敌人，方便统一管理（例如动画）
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        // 订阅光照更新事件
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnEnable()
    {
        // 订阅 ARPlaneManager 的 planesChanged 事件
        if (planeManager != null)
            planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        // 取消订阅
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnDestroy()
    {
        // 取消订阅光照
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void Update()
    {
        // 用户点击屏幕时，从摄像机中心发射射线，检测是否击中敌人
        HandleTouchInput();

        // 给已生成的敌人做简单动画
        AnimateEnemies();
    }

    /// <summary>
    /// 当检测到新平面时，自动在该平面中心生成一个敌人
    /// </summary>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
        {
            // 取第一个新增平面
            ARPlane plane = args.added[0];
            Vector3 spawnPos = plane.transform.position;

            SpawnEnemy(spawnPos);
        }
    }

    /// <summary>
    /// 生成敌人
    /// </summary>
    private void SpawnEnemy(Vector3 position)
    {
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
            Debug.Log($"Spawned an enemy at {position}");
        }
        else
        {
            Debug.LogWarning("Enemy Prefab is not assigned!");
        }
    }

    /// <summary>
    /// 屏幕触摸检测：只要用户点击屏幕，就从摄像机中心向前发射射线，若击中敌人则销毁
    /// </summary>
    private void HandleTouchInput()
    {
        // 如果在编辑器模式下，不处理或可用鼠标模拟
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // 若需要排除 UI 点击，可在此处判断
            ShootFromCameraCenter();
        }
#endif

        // 在真机上处理触摸
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // 如果点击到 UI，则忽略
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                ShootFromCameraCenter();
            }
        }
    }

    /// <summary>
    /// 从摄像机中心向前发射射线，如果击中 Enemy 则销毁
    /// </summary>
    private void ShootFromCameraCenter()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 从摄像机位置向摄像机正前方发射射线
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Enemy destroyed at " + hit.point);
            }
        }
    }

    /// <summary>
    /// 光照估计：如果需要，可以用来调整场景灯光
    /// </summary>
    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            float brightness = args.lightEstimation.averageBrightness.Value;
            // 在此可根据 brightness 调整场景灯光
            // e.g. myLight.intensity = brightness;
        }
    }

    /// <summary>
    /// 简单动画：让敌人左右摆动
    /// </summary>
    private void AnimateEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                float speed = 0.2f;
                Vector3 pos = enemy.transform.position;
                pos.x += Mathf.Sin(Time.time) * speed * Time.deltaTime;
                enemy.transform.position = pos;
            }
        }
    }
}
