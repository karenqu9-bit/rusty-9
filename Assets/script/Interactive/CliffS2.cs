using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class CliffS2 : Interactive
{
    [Header("Dialogue Data")]
    public DialogueData_SO dialoguePrank;
    public DialogueData_SO dialogueGhost;
    public DialogueData_SO dialogueBully;

    [Header("UI Position Settings")]
    public Vector2 customDialoguePosition = new Vector2(0, -150);

    private DialogueController dialogueController;
    private DialogueUI dialogueUI;
    private ItemName lastSelectedItem = ItemName.None;

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (isSelected && itemDetails != null)
        {
            lastSelectedItem = itemDetails.itemName;
        }
        else if (!isSelected)
        {
            lastSelectedItem = ItemName.None;
        }
    }

    public override void EmptyClicked()
    {
        HandleDialogue();
    }

    public override void CheckItem(ItemName itemName)
    {
        // 【核心逻辑修改】

        // 1. 如果手持的是 Bear，则触发交互（移除物品 + 显示对话）
        if (lastSelectedItem == ItemName.Bear)
        {
            // 通知 InventoryManager 移除 Bear
            EventHandler.CallItemUsedEvent(lastSelectedItem);
            // 2. 【改进】广播事件：告诉全世界“我把熊给了 Cliff”
            EventHandler.CallItemGivenToNPCEvent(ItemName.Bear);

            // 显示 Bear 对应的对话
            HandleDialogue();
            return; // 处理完直接返回，不再执行下面的逻辑
        }

        // 2. 如果手持的是其他物品（Glass, Mask 等），则“不发生交互”
        // 也就是：不移除物品，也不显示对话框。
        // 如果你想显示一句“这东西对 Cliff 没用”，可以在这里加 else if 判断并显示提示对话。
        // 目前按你的要求：什么都不做。

        Debug.Log($"[CliffS2] Holding {lastSelectedItem}, but no interaction defined for this item with Cliff.");
    }

    private void HandleDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetPanelPosition(customDialoguePosition);
        }

        // 注意：这里的逻辑依赖 lastSelectedItem

        // 再次确认是 Bear (虽然 CheckItem 里已经判断过，但为了代码健壮性保留)
        if (lastSelectedItem == ItemName.Bear)
        {
            if (dialogueBully != null)
            {
                dialogueController.ShowDynamicDialogue(dialogueBully);
            }
            return;
        }

        // 以下是空手点击时的逻辑
        if (InventoryManager.Instance.HasItem(ItemName.Mask))
        {
            if (dialogueGhost != null)
            {
                dialogueController.ShowDynamicDialogue(dialogueGhost);
            }
            return;
        }

        if (dialoguePrank != null)
        {
            dialogueController.ShowDynamicDialogue(dialoguePrank);
        }
    }
}