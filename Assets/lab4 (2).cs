using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.XR.ARSubsystems;

public class ARUnitLab : MonoBehaviour
{

    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private TMP_Text _planeText;
    [SerializeField] private ARPlaneManager _arPlaneManager;
    [SerializeField] private ARPointCloudManager _arPointCloudManager;
    [SerializeField] private ARRaycastManager _arRaycastManager;
    [SerializeField] private GameObject _robotPrefab;

    private List<ARPlane> _activePlanes = new List<ARPlane>();
    private List<ARPointCloud> _activePointClouds = new List<ARPointCloud>();

    // Store raycast hits
    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

    // GameObject tobe spawned; null at first
    private GameObject _spawnedObject = null;

    // Start is called before the first frame update
    void Start()
    {
        
        // Add callbacks
        ARSession.stateChanged += OnARSessionStateChanged;
        _arPlaneManager.planesChanged += OnPlanesChanged;
        _arPointCloudManager.pointCloudsChanged += OnPointCloudsChanged;
    }

    // Update is called once per frame
    void Update()
    {

        // handle texts
        int numOfPoints = 0;
        foreach (ARPointCloud cloud in _activePointClouds)
        {
            numOfPoints = numOfPoints + cloud.identifiers.Value.Length;
        }
        _planeText.text = $"% planes: {_activePlanes.Count}\n % points: {numOfPoints}";


        // handle screen touches
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (_arRaycastManager.Raycast(touch.position, _raycastHits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = _raycastHits[0].pose;
                // if a robot has not been instantiated in the scene
                if (_spawnedObject == null)
                {
                    _spawnedObject = GameObject.Instantiate(_robotPrefab, hitPose.position, hitPose.rotation);
                }
                else {
                    // Object already instantiated. Move it at touch position 
                    _spawnedObject.transform.position = hitPose.position;
                }
            }
        }
    }

    private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        _stateText.text = args.state.ToString();
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // handle added planes
        foreach (ARPlane plane in args.added)
        {
            if (!_activePlanes.Contains(plane))
            {
                _activePlanes.Add(plane);
            }
        }

        // handle removed planes
        foreach (ARPlane plane in args.removed)
        {
            if (_activePlanes.Contains(plane))
            {
                _activePlanes.Remove(plane);
            }
        }

        // handle merged planes
        foreach (ARPlane plane in args.updated)
        {
            if (plane.subsumedBy != null && _activePlanes.Contains(plane.subsumedBy))
            {
                _activePlanes.Remove(plane);
            }
            else if (plane.subsumedBy == null && !_activePlanes.Contains(plane))
            {
                _activePlanes.Add(plane);
            }
        }
    }

    private void OnPointCloudsChanged(ARPointCloudChangedEventArgs args)
    {
        // handle added clouds
        foreach (ARPointCloud cloud in args.added)
        {
            if (!_activePointClouds.Contains(cloud))
            {
                _activePointClouds.Add(cloud);
            }
        }

        // handle removed clouds
        foreach (ARPointCloud cloud in args.removed)
        {
            if (_activePointClouds.Contains(cloud))
            {
                _activePointClouds.Remove(cloud);
            }
        }

        // handle updated clouds
        foreach (ARPointCloud cloud in args.updated)
        {
            if (_activePointClouds.Contains(cloud))
            {
                int index = _activePointClouds.IndexOf(cloud);
                if (index != -1)
                {
                    _activePointClouds[index] = cloud;
                }
            }
        }
    }
}



