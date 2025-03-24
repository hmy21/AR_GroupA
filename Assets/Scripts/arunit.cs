using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class arunit : MonoBehaviour
{

    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject robotPrefab;
    private Transform trackedImageTransform = null;
    private Vector3 tracked_position;
    private Quaternion tracked_rotation;
    private GameObject robot = null;
    private float count = 1.0f;

    void OnEnable(){
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable(){
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Update the object position here
        robot.transform.position = tracked_position;
        robot.transform.rotation = tracked_rotation;
        float count_norm = count / 600.0f;
        float angle = count_norm * 360.0f;
        float radian = count_norm * 2.0f * Mathf.PI;
        robot.transform.Translate(0.1f * Mathf.Sin(radian), 0.0f, 0.1f * Mathf.Cos(radian));
        robot.transform.Rotate(0.0f, angle + 180.0f, 0.0f);
        count += 1.0f;
        if (count >= 600.0f)
            count -= 600.0f;
        Debug.Log($"Current robot transform: {robot.transform.position}, {robot.transform.rotation}");
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            robot = GameObject.Instantiate(robotPrefab, newImage.transform.position, newImage.transform.rotation);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
            tracked_position = updatedImage.transform.position;
            tracked_rotation = updatedImage.transform.rotation;
            Debug.Log($"updatedImage.transform: {updatedImage.transform.position}, {updatedImage.transform.rotation}");
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }
    }
}
