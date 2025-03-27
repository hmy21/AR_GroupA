using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class LandmineSpawner : MonoBehaviour
{
    [Header("Landmine Placement Settings")]
    [Tooltip("����Ԥ�Ƽ�������������׿������ű�")]
    public GameObject landminePrefab;

    [Tooltip("�Ƿ������η��õ��ף����Ϊ false����ֻ�������һ��")]
    public bool allowMultiplePlacements = true;

    [Tooltip("���������η���ʱ������������")]
    public int maxPlacements = 1;

    [Header("AR Settings")]
    [Tooltip("����ƽ����� ARRaycastManager����δ��ֵ���Զ����ң�")]
    public ARRaycastManager arRaycastManager;

    // �ڲ��洢 ARRaycast �����
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // ��¼�ѷ��õĵ�������
    private int placementsCount = 0;

    [Header("Placement Control")]
    [Tooltip("ֻ�е��˱�־Ϊ true ʱ����������õ���")]
    public bool isPlacementEnabled = false;

    void Awake()
    {
        if (arRaycastManager == null)
        {
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (arRaycastManager == null)
            {
                Debug.LogError("δ�ڳ������ҵ� ARRaycastManager����ȷ����������һ�� ARRaycastManager �����");
            }
        }
    }

    void Update()
    {


        // �ڴ�����ʼʱ��������߼�
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
                    Debug.Log("δ��⵽ƽ�档");
                }
            }
        }
    }
}
