using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 确保挂载此脚本的对象也有 DialogueController
[RequireComponent(typeof(DialogueController))]
public class Gemma : Interactive
{
    [Header("Dialogue Data")]
    public DialogueData_SO dialogueData; // 拖入包含对话内容的 SO 资产

    [Header("UI Position Settings")]
    public Vector2 customDialoguePosition = new Vector2(0, -150); // 可根据需要调整位置

    private DialogueController dialogueController;
    private DialogueUI dialogueUI;

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        // 查找场景中的 DialogueUI 实例
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    /// <summary>
    /// 当玩家空手点击 Gemma 时触发
    /// </summary>
    public override void EmptyClicked()
    {
        HandleInteraction();
    }

    /// <summary>
    /// 如果以后需要手持物品交互，可以重写此方法
    /// 目前暂时复用 EmptyClicked 的逻辑，或者根据需求扩展
    /// </summary>
    public override void CheckItem(ItemName itemName)
    {
        // 如果手持物品点击也想显示同样的对话，取消下面注释
        HandleInteraction();
        
        // 如果手持物品点击有特殊逻辑，可以在这里添加
        // 例如：如果手持某个特定物品，显示不同对话
    }

    /// <summary>
    /// 处理具体的对话显示逻辑
    /// </summary>
    private void HandleInteraction()
    {
        // 1. 设置对话框位置
        if (dialogueUI != null)
        {
            dialogueUI.SetPanelPosition(customDialoguePosition);
        }

        // 2. 显示对话
        if (dialogueData != null)
        {
            dialogueController.ShowDynamicDialogue(dialogueData);
        }
        else
        {
            Debug.LogWarning("[Gemma] Dialogue Data is not assigned in Inspector!");
        }
    }
}