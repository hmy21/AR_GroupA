using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARGameController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARCameraManager cameraManager;
    [SerializeField] private ARRaycastManager raycastManager;

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnEnable()
    {
        if (planeManager != null)
            planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnDestroy()
    {
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void Update()
    {
        AnimateEnemies();
    }

    /// <summary>
    /// 当检测到新平面时，在平面中心生成一个敌人（可选）
    /// </summary>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
        {
            ARPlane plane = args.added[0];
            Vector3 spawnPos = plane.transform.position;
            SpawnEnemy(spawnPos);
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
            Debug.Log($"Spawned an enemy at {position}");
        }
    }

    /// <summary>
    /// 光照估计（可选）
    /// </summary>
    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            float brightness = args.lightEstimation.averageBrightness.Value;
            // 你可以根据 brightness 调整灯光强度
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
