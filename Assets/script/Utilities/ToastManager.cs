using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // 引入 TMP 命名空间

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject toastPanel; // 拖入 ToastPanel
    public TMP_Text toastText;         // 拖入 ToastText (如果是TMP，改为 TMPro.TextMeshProUGUI)

    [Header("Settings")]
    public float displayDuration = 2f; // 显示时长

    private Coroutine currentCoroutine;

    private void Awake()
    {
        // 单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // 确保初始是隐藏的
        if (toastPanel != null)
            toastPanel.SetActive(false);
    }

    /// <summary>
    /// 外部调用此方法显示提示
    /// </summary>
    public void ShowToast(string message)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string msg)
    {
        // 1. 显示面板并设置文字
        if (toastPanel != null) toastPanel.SetActive(true);
        if (toastText != null) toastText.text = msg;

        // 2. 等待指定时间
        yield return new WaitForSeconds(displayDuration);

        // 3. 隐藏面板
        if (toastPanel != null) toastPanel.SetActive(false);
        
        currentCoroutine = null;
    }
}