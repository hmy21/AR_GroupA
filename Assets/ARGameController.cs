using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARGameController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARPlaneManager planeManager;       // ���ƽ��
    [SerializeField] private ARCameraManager cameraManager;     // ��ȡ���չ���
    [SerializeField] private ARRaycastManager raycastManager;   // ����Ҫ��������ƽ�棬������

    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;            // ����Ԥ�Ƽ�

    // �洢���ɵĵ��ˣ�����ͳһ�������綯����
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        // ���Ĺ��ո����¼�
        if (cameraManager != null)
            cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnEnable()
    {
        // ���� ARPlaneManager �� planesChanged �¼�
        if (planeManager != null)
            planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnDisable()
    {
        // ȡ������
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }

    private void OnDestroy()
    {
        // ȡ�����Ĺ���
        if (cameraManager != null)
            cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    void Update()
    {
        // �û������Ļʱ������������ķ������ߣ�����Ƿ���е���
        HandleTouchInput();

        // �������ɵĵ������򵥶���
        AnimateEnemies();
    }

    /// <summary>
    /// ����⵽��ƽ��ʱ���Զ��ڸ�ƽ����������һ������
    /// </summary>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added != null && args.added.Count > 0)
        {
            // ȡ��һ������ƽ��
            ARPlane plane = args.added[0];
            Vector3 spawnPos = plane.transform.position;

            SpawnEnemy(spawnPos);
        }
    }

    /// <summary>
    /// ���ɵ���
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
    /// ��Ļ������⣺ֻҪ�û������Ļ���ʹ������������ǰ�������ߣ������е���������
    /// </summary>
    private void HandleTouchInput()
    {
        // ����ڱ༭��ģʽ�£��������������ģ��
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // ����Ҫ�ų� UI ��������ڴ˴��ж�
            ShootFromCameraCenter();
        }
#endif

        // ������ϴ�����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // �������� UI�������
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                ShootFromCameraCenter();
            }
        }
    }

    /// <summary>
    /// �������������ǰ�������ߣ�������� Enemy ������
    /// </summary>
    private void ShootFromCameraCenter()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // �������λ�����������ǰ����������
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
    /// ���չ��ƣ������Ҫ�������������������ƹ�
    /// </summary>
    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            float brightness = args.lightEstimation.averageBrightness.Value;
            // �ڴ˿ɸ��� brightness ���������ƹ�
            // e.g. myLight.intensity = brightness;
        }
    }

    /// <summary>
    /// �򵥶������õ������Ұڶ�
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
