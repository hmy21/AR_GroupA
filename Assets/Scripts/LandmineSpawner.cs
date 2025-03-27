using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class LandmineSpawner : MonoBehaviour
{
    [Header("Landmine Placement Settings")]
    [Tooltip("地雷预制件，必须包含地雷控制器脚本")]
    public GameObject landminePrefab;

    [Tooltip("是否允许多次放置地雷；如果为 false，则只允许放置一次")]
    public bool allowMultiplePlacements = true;

    [Tooltip("当不允许多次放置时，最大放置数量")]
    public int maxPlacements = 1;

    [Header("AR Settings")]
    [Tooltip("用于平面检测的 ARRaycastManager（若未赋值将自动查找）")]
    public ARRaycastManager arRaycastManager;

    // 内部存储 ARRaycast 检测结果
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // 记录已放置的地雷数量
    private int placementsCount = 0;

    [Header("Placement Control")]
    [Tooltip("只有当此标志为 true 时，才允许放置地雷")]
    public bool isPlacementEnabled = false;

    void Awake()
    {
        if (arRaycastManager == null)
        {
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (arRaycastManager == null)
            {
                Debug.LogError("未在场景中找到 ARRaycastManager，请确保场景中有一个 ARRaycastManager 组件。");
            }
        }
    }

    void Update()
    {


        // 在触摸开始时处理放置逻辑
        if (Input.touchCount > 0 && isPlacementEnabled)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Vector2 touchPosition = touch.position;
                if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    if (!allowMultiplePlacements && placementsCount >= maxPlacements)
                        return;

                    Instantiate(landminePrefab, hitPose.position, hitPose.rotation);
                    placementsCount++;
                    Debug.Log("Placed landmine at: " + hitPose.position);
                }
                else
                {
                    Debug.Log("未检测到平面。");
                }
            }
        }
    }
}
