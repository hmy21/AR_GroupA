using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.Mathematics;

public class EnemyGenerationTrack : MonoBehaviour
{
    [SerializeField] private TMP_Text _stateText;
    [SerializeField] private TMP_Text _planeText;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject GenerationPointPrefabs;
    [SerializeField] private GameObject BlockPrefabs;//todo
    private Vector3 tracked_position;
    private Quaternion tracked_rotation;
    private GameObject GenerationPoint;
    private Dictionary<ARTrackedImage, GameObject> TrackedImage = new Dictionary<ARTrackedImage, GameObject>();
    private Dictionary<ARTrackedImage, Vector3> tracked_positions  = new Dictionary<ARTrackedImage, Vector3>();
    private Dictionary<ARTrackedImage, Quaternion> tracked_rotations = new Dictionary<ARTrackedImage, Quaternion>();

    private GameObject testOBJ;


    void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void Update()
    {
        // Vector3 postion = new Vector3(0, 0, 0);
        // Quaternion rotation = new Quaternion(0,0,0,0);
        foreach (var image in TrackedImage)
        {
            image.Value.transform.position = tracked_positions[image.Key];
            image.Value.transform.rotation = tracked_rotations[image.Key];

        }
        // postion = tracked_positions[image.Key];
        // rotation = tracked_rotations[image.Key];
        // _stateText.text = "true";
        // _planeText.text = $"%position:{postion}, rotaion:{rotation}";

    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            // string imageName = newImage.referenceImage.name;

            GameObject newGenerationPoint = Instantiate(GenerationPointPrefabs, newImage.transform.position, newImage.transform.rotation);
            TrackedImage[newImage] = newGenerationPoint;

        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event

            tracked_positions[updatedImage] = updatedImage.transform.position;
            tracked_rotations[updatedImage] = updatedImage.transform.rotation;

        }

        foreach (var removedImage in eventArgs.removed)
        {
            // if (TrackedImage.TryGetValue(removedImage, out GameObject removedGenerationPoint))
            // {
            //     Destroy(removedGenerationPoint);
            //     TrackedImage.Remove(removedImage);
            //     tracked_positions.Remove(removedImage);
            //     tracked_rotations.Remove(removedImage);
            // }
        }
    }


}
