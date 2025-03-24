using UnityEngine;
using UnityEngine.EventSystems;
using BigRookGames.Weapons;

public class FullScreenDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("准星的 RectTransform")]
    public RectTransform crosshairRect;

    [Tooltip("枪械控制器脚本，用于开火")]
    public GunfireController gunController;

    [Tooltip("轻点判定的移动阈值（像素）")]
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
            // 拖拽准星
            crosshairRect.anchoredPosition += eventData.delta / (GetComponentInParent<Canvas>().scaleFactor);
            isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float distance = (eventData.position - pointerDownPos).magnitude;
        Debug.Log($"OnPointerUp invoked, distance: {distance}");
        // 如果移动距离小于阈值，认为是轻点
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
