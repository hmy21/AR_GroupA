using UnityEngine;
using UnityEngine.EventSystems;
using BigRookGames.Weapons;

public class FullScreenDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("׼�ǵ� RectTransform")]
    public RectTransform crosshairRect;

    [Tooltip("ǹе�������ű������ڿ���")]
    public GunfireController gunController;

    [Tooltip("����ж����ƶ���ֵ�����أ�")]
    public float tapThreshold = 10f;

    private Vector2 pointerDownPos;
    private bool isDragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPos = eventData.position;
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (crosshairRect != null)
        {
            // ��ק׼��
            crosshairRect.anchoredPosition += eventData.delta / (GetComponentInParent<Canvas>().scaleFactor);
            isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float distance = (eventData.position - pointerDownPos).magnitude;
        Debug.Log($"OnPointerUp invoked, distance: {distance}");
        // ����ƶ�����С����ֵ����Ϊ�����
        if (distance < tapThreshold)
        {
            Debug.Log("Tap detected, triggering FireWeapon()");
            if (gunController != null && gunController.enabled)
            {
                gunController.FireWeapon();
            }
        }

    }
}
