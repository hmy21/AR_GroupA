using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceBattlefield : MonoBehaviour
{
    public GameObject battlefieldPrefab; // 6.3m x 7.9m 的战场预制体
    private ARPlaneManager planeManager; // 用来管理 AR 平面的 ARPlaneManager
    private GameObject spawnedBattlefield; // 用来存储生成的战场对象
    private bool battlefieldPlaced = false;

    void Start()
    {
        // 获取 ARPlaneManager 组件
        planeManager = GetComponent<ARPlaneManager>();
        
        // 确保 ARPlaneManager 组件已启用
        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager not found on this GameObject.");
        }
    }

    void Update()
    {
        // 如果战场已放置，则返回
        if (battlefieldPlaced) return;

        // 检查是否有检测到平面
        if (planeManager.trackables.count > 0)
        {
            // 获取第一个检测到的平面
            foreach (ARPlane plane in planeManager.trackables)
            {
                // 使用平面的中心位置
                Pose hitPose = new Pose(plane.center, Quaternion.identity);

                if (spawnedBattlefield == null) 
                {
                    // 在平面上放置战场
                    spawnedBattlefield = Instantiate(battlefieldPrefab, hitPose.position, hitPose.rotation);
                    battlefieldPlaced = true; // 标记战场已放置
                    break; // 只放置一次战场，跳出循环
                }
            }
        }
    }
}