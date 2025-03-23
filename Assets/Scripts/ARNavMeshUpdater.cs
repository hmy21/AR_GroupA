using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class ARNavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    private ARPlaneManager arPlaneManager;

    private bool navMeshBuilt = false;

    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>();

        if (arPlaneManager != null)
        {
            arPlaneManager.planesChanged += OnFirstPlaneDetected;
        }
    }

    void OnFirstPlaneDetected(ARPlanesChangedEventArgs args)
    {
        if (navMeshBuilt) return;

        if (args.added.Count > 0)
        {
            // 只用第一个平面
            ARPlane selectedPlane = args.added[0];

            // 关闭其他平面
            foreach (var plane in arPlaneManager.trackables)
            {
                if (plane != selectedPlane)
                    plane.gameObject.SetActive(false);
            }

            // 添加 MeshCollider
            if (!selectedPlane.TryGetComponent(out MeshCollider col))
                selectedPlane.gameObject.AddComponent<MeshCollider>();

            // 动态烘焙 NavMesh
            navMeshSurface.BuildNavMesh();

            // 只构建一次
            navMeshBuilt = true;

            // 停止监听后续平面变化
            arPlaneManager.planesChanged -= OnFirstPlaneDetected;

            // 可选：关闭 AR 平面检测，防止继续探测
            arPlaneManager.enabled = false;
        }
    }
}