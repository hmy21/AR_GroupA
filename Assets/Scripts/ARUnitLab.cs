using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.Collections;

using TMPro;

public class ARUnitLab : MonoBehaviour
{
    int plane_count = 0;
    int point_count = 0;
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private TMP_Text _planeText;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private ARPointCloudManager _arPointCloudManager;
    private HashSet<ARPlane> _subsumedPlanes = new HashSet<ARPlane>();

    // Start is called before the first frame update
    void Start()
    {
        ARSession.stateChanged += OnARSessionStateChanged;
        _arPlaneManager.planesChanged += OnARPlanesChanged;
    }

    // Update is called once per frame
    void Update()
    {
        // plane_count can be obtained directly from trackables.count
        // plane_count = _arPlaneManager.trackables.count;

        // point_count has to be computed by accummulate the points from all pointclouds
        int point_count = 0;
        foreach (var trackable in _arPointCloudManager.trackables)
        {
            if (trackable.positions is NativeSlice<Vector3> non_null_positions) point_count += non_null_positions.Length;
        }

        // Update the plane text
        _planeText.text = plane_count + " Planes" + "\n" + point_count + " Points";
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        // Update the state text
        _stateText.text = args.ToString();
    }

    private void OnARPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // plane_count can also be computed by getting the changes from the event handler
        // plane_count += args.added.Count - args.removed.Count;

        plane_count += args.added.Count;

        plane_count -= args.removed.Count;

        foreach (var updatedPlane in args.updated)
        {
            if (updatedPlane.subsumedBy != null)
            {
                if (!_subsumedPlanes.Contains(updatedPlane))
                {
                    plane_count--; // 立即减少被合并的平面
                    _subsumedPlanes.Add(updatedPlane); // 记录它，防止在 removed 里再次处理
                }
            }
        }
         _subsumedPlanes.RemoveWhere(plane => args.removed.Contains(plane));
        

    }
}