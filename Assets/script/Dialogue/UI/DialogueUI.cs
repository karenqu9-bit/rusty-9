using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public Text dialogueText;
    
    // 记录初始位置作为默认位置
    private Vector2 defaultPosition;

    private void Start()
    {
        if (panel != null)
        {
            defaultPosition = panel.GetComponent<RectTransform>().anchoredPosition;
        }
    }

    private void OnEnable()
    {
        // 监听对话显示事件
        EventHandler.ShowDialogueEvent += ShowDialogue;
        
        // 【新增】监听场景卸载事件，确保切换场景时关闭对话框
        EventHandler.BeforeSceneUnloadEvent += HideDialogueOnSceneChange;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= ShowDialogue;
        
        // 【新增】取消订阅，防止内存泄漏
        EventHandler.BeforeSceneUnloadEvent -= HideDialogueOnSceneChange;
    }

    /// <summary>
    /// 【新增】当场景即将卸载时，强制隐藏对话框
    /// </summary>
    private void HideDialogueOnSceneChange()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        // 可选：清空文本，防止下一场景加载瞬间看到上一句对话
        if (dialogueText != null)
        {
            dialogueText.text = string.Empty;
        }
        
        // 重置位置，防止影响下一场景
        ResetPanelPosition();
    }

    /// <summary>
    /// 【新增】公开方法：允许外部脚本修改对话框位置
    /// </summary>
    /// <param name="newPos">新的锚点位置</param>
    public void SetPanelPosition(Vector2 newPos)
    {
        if (panel != null)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = newPos;
            }
        }
    }

    // 新增：重置为默认位置
    public void ResetPanelPosition()
    {
        SetPanelPosition(defaultPosition);
    }
    
    private void ShowDialogue(string dialogue)
    {
        if (dialogue != string.Empty)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
            // 对话框关闭时，重置位置，防止影响下一次其他NPC的对话
            ResetPanelPosition(); 
        }
        
        dialogueText.text = dialogue;
    }
}