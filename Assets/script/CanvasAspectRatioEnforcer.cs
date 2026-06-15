using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasAspectRatioEnforcer : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    private const float TargetAspectRatio = 16f / 9f; // 1.77778

    void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        ApplyEnforcement();
    }

#if UNITY_EDITOR
    void Update()
    {
        // 方便你在编辑器里拉伸 Game 窗口时实时预览
        ApplyEnforcement();
    }
#endif

    void ApplyEnforcement()
    {
        if (canvasScaler == null) return;

        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (currentAspectRatio > TargetAspectRatio)
        {
            // 手机横屏（超宽屏），网页端 canvas 高度已填满
            // 此时 UI 匹配高度（1f），能确保游戏内容完美居中不裁剪
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            // 手机竖屏或者 iPad 等屏幕
            // 此时网页端 canvas 宽度已填满。为了防止 UI 元素被左右裁剪，
            // 强烈建议将竖屏模式的匹配完全倾向于 宽度 (0f)，而不是 0.5f！
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}