using UnityEngine;
using UnityEngine.XR.ARFoundation; // 导入 ARFoundation 功能
using TMPro; // 导入 TextMeshPro 功能
using System.Collections.Generic; // 用于 List 管理

public class ARUnitLab : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateText; // 用于显示 AR 会话状态
    [SerializeField] private TMP_Text _planeText; // 用于显示平面数量的文本
    [SerializeField] private ARPlaneManager _arPlaneManager; // 用于管理 AR 平面

    private List<ARPlane> _trackedPlanes = new List<ARPlane>(); // 存储检测到的平面

    private void Start()
    {
        // 注册 ARSession 状态变化事件
        ARSession.stateChanged += OnARSessionStateChanged;

        // 注册 ARPlaneManager 的平面变化事件
        if (_arPlaneManager != null)
        {
            _arPlaneManager.planesChanged += OnPlanesChanged;
        }
        else
        {
            Debug.LogError("ARPlaneManager is not assigned in the Inspector!");
        }
    }

    private void OnDestroy()
    {
        // 注销 ARSession 状态变化事件
        ARSession.stateChanged -= OnARSessionStateChanged;

        // 注销 ARPlaneManager 平面变化事件
        if (_arPlaneManager != null)
        {
            _arPlaneManager.planesChanged -= OnPlanesChanged;
        }
    }

    /// <summary>
    /// 更新 AR 会话状态到 UI
    /// </summary>
    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // 显示 AR 会话状态
        _stateText.text = $"AR State: {args.state}";
        Debug.Log($"AR Session State Changed: {args.state}");
    }

    /// <summary>
    /// 当检测到平面变化时触发
    /// </summary>
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // 更新检测到的平面列表
        foreach (var plane in args.added)
        {
            if (!_trackedPlanes.Contains(plane))
            {
                _trackedPlanes.Add(plane);
            }
        }

        // 移除不再被跟踪的平面
        foreach (var plane in args.removed)
        {
            _trackedPlanes.Remove(plane);
        }

        // 更新 UI 显示检测到的平面数量
        _planeText.text = $"Detected Planes: {_trackedPlanes.Count}";
        Debug.Log($"Planes Added: {args.added.Count}, Updated: {args.updated.Count}, Removed: {args.removed.Count}");
    }
}
