using UnityEngine;
using UnityEngine.EventSystems;
using BigRookGames.Weapons;


public class FullScreenDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("׼�ǵ� RectTransform")]
    public RectTransform crosshairRect;

    [Tooltip("���ڴ��������ǹе������")]
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
            // ����׼��λ�ã����� Canvas ��������
            crosshairRect.anchoredPosition += eventData.delta / (GetComponentInParent<Canvas>().scaleFactor);
            isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ������ָ���ƶ�����
        float distance = (eventData.position - pointerDownPos).magnitude;
        if (distance < tapThreshold)
        {
            // ��Ϊ����㣬����ǹе����
            if (gunController != null)
            {
                gunController.FireWeapon();
            }
        }
    }
}
