using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class LandmineSpawner : MonoBehaviour
{
    [Header("Landmine Placement Settings")]
    [Tooltip("����Ԥ�Ƽ���������� LandmineController �ű�")]
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

    void Awake()
    {
        // �Զ����ҳ����е� ARRaycastManager�������ֶ���ק
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
        // ���ڴ�����ʼʱ���м��
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // ������������� UI �������
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                Vector2 touchPosition = touch.position;
                if (arRaycastManager != null && arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                    // ����������η��ã����ѷ��ô����ﵽ���ޣ��򲻷���
                    if (!allowMultiplePlacements && placementsCount >= maxPlacements)
                        return;

                    // �ڼ�⵽��ƽ��λ�����ɵ���Ԥ�Ƽ�
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
