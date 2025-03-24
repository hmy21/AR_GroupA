using UnityEngine;

public class UIPositioner : MonoBehaviour
{
    public RectTransform pauseButton;

    void Start()
    {
        // 设置锚点和位置
        pauseButton.anchorMin = new Vector2(0, 1); // 左上角
        pauseButton.anchorMax = new Vector2(0, 1);
        pauseButton.pivot = new Vector2(0, 1);
        
        // 设置相对位置 (距离左侧 10px, 距离顶部 10px)
        pauseButton.anchoredPosition = new Vector2(10, -10);
    }
}
