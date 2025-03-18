using UnityEngine;
using UnityEngine.EventSystems;
using BigRookGames.Weapons;


public class FullScreenDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("准星的 RectTransform")]
    public RectTransform crosshairRect;

    [Tooltip("用于触发开火的枪械控制器")]
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
            // 更新准星位置，考虑 Canvas 缩放因子
            crosshairRect.anchoredPosition += eventData.delta / (GetComponentInParent<Canvas>().scaleFactor);
            isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 计算手指总移动距离
        float distance = (eventData.position - pointerDownPos).magnitude;
        if (distance < tapThreshold)
        {
            // 认为是轻点，触发枪械开火
            if (gunController != null)
            {
                gunController.FireWeapon();
            }
        }
    }
}
