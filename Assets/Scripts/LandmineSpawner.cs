using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class LandmineSpawner : MonoBehaviour
{
    [Header("Landmine Placement Settings")]
    [Tooltip("地雷预制件，必须包含 LandmineController 脚本")]
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

    void Awake()
    {
        // 自动查找场景中的 ARRaycastManager，无需手动拖拽
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
        // 仅在触摸开始时进行检测
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // 如果触摸发生在 UI 上则忽略
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Vector2 touchPosition = touch.position;
                if (arRaycastManager != null && arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    // 如果不允许多次放置，且已放置次数达到上限，则不放置
                    if (!allowMultiplePlacements && placementsCount >= maxPlacements)
                        return;

                    // 在检测到的平面位置生成地雷预制件
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
