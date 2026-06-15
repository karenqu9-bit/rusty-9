using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    void Start()
    {
        // 目标比例 16:9
        float targetAspect = 16.0f / 9.0f;

        // 当前实际窗口的比例
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 计算缩放比例
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        // 如果实际比例更宽，则在左右加黑边
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        // 如果实际比例更高，则在上下加黑边
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }
}