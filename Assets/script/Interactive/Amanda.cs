using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class Amanda : Interactive
{
    private BoxCollider2D coll;

    private DialogueController dialogueController;

    // 【新增】获取 DialogueUI 的引用
    private DialogueUI dialogueUI;

    [Header("UI Position Settings")]
    // 你可以在 Inspector 中调整这个值
    public Vector2 customDialoguePosition = new Vector2(-270, -150);

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        // spriteRenderer = GetComponent<SpriteRenderer>(); //
        coll = GetComponent<BoxCollider2D>();

        // 【关键】在 Awake 或 Start 中查找 DialogueUI 实例
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (isDone)
        {
            // coll.enabled = false; //
        }
    }

    public override void EmptyClicked()
    {
        // 【关键】在显示对话前，先设置位置
        SetDialoguePosition();

        if (isDone)
            dialogueController.ShowDialogueFinish();
        else
            //对话内容A
            dialogueController.ShowDialogueEmpty();
    }

    protected override void OnclickedAction()
    {
        Debug.Log("[Amanda] OnclickedAction Triggered! Calling Event..."); // 【新增】
        // 【关键】在显示对话前，先设置位置
        SetDialoguePosition();

        dialogueController.ShowDialogueFinish();
        EventHandler.CallItemGivenToNPCEvent(ItemName.Mask);
        Debug.Log("[Amanda] Event Called with ItemName.Mask");
    }

    // 【新增】封装设置位置的方法，避免重复代码
    private void SetDialoguePosition()
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetPanelPosition(customDialoguePosition);
        }
        else
        {
            Debug.LogWarning("[Amanda] DialogueUI not found! Make sure DialogueUI exists in the scene.");
        }
    }
}