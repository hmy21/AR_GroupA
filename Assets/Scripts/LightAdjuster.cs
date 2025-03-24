using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering;

public class LightAdjuster : MonoBehaviour
{
    public Light arLight;  // 绑定AR环境中的灯光
    private ARCameraManager arCameraManager;

    void Start()
    {
        arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    // 处理光线变化
    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            float brightness = args.lightEstimation.averageBrightness.Value;
            AdjustLight(brightness);
        }
    }

    // 调节光源亮度
    private void AdjustLight(float brightness)
    {
        // 亮度阈值
        float minBrightness = 0.2f; // 环境变暗时
        float maxIntensity = 3.0f;  // 最大光强

        // 根据环境亮度调节灯光强度
        if (brightness < minBrightness)
        {
            arLight.intensity = Mathf.Lerp(arLight.intensity, maxIntensity, Time.deltaTime * 2.0f);
        }
        else
        {
            arLight.intensity = Mathf.Lerp(arLight.intensity, brightness, Time.deltaTime * 2.0f);
        }
    }

    private void OnDestroy()
    {
        if (arCameraManager != null)
        {
            arCameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }
}
