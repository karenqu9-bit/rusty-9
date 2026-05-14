using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class TalbertS1 : Interactive
{
    private SpriteRenderer spriteRenderer;
    private DialogueController dialogueController;

    [Header("Sprites")]
    public Sprite holdingDollSprite;
    public Sprite emptyHandedSprite;
    public Sprite openSprite;

    // 【移除】不再需要 private bool hasDoll;
    // 我们直接用 InventoryManager 的状态来判断

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 场景初始化时更新一次
        UpdateSpriteBasedOnState();
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
        // 每次场景加载后，根据背包状态刷新图片
        UpdateSpriteBasedOnState();
    }

    // 【新增】根据全局状态决定显示哪张图
    private void UpdateSpriteBasedOnState()
    {
        if (isDone)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            // 核心逻辑：如果背包里有 Doll，说明已经被拿走了 -> 显示空手
            // 如果背包里没有 Doll，说明还没拿 -> 显示抱着娃娃
            bool hasDollInInventory = InventoryManager.Instance.HasItem(ItemName.Doll);
            
            if (hasDollInInventory)
            {
                if (emptyHandedSprite != null) spriteRenderer.sprite = emptyHandedSprite;
            }
            else
            {
                if (holdingDollSprite != null) spriteRenderer.sprite = holdingDollSprite;
            }
        }
    }

    public override void EmptyClicked()
    {
        if (isDone)
        {
            dialogueController.ShowDialogueFinish();
            return;
        }

        // 同样利用 InventoryManager 判断
        if (!InventoryManager.Instance.HasItem(ItemName.Doll))
        {
            TakeDoll();
        }
        else
        {
            dialogueController.ShowDialogueEmpty();
        }
    }

    public override void CheckItem(ItemName itemName)
    {
        if (isDone)
        {
            dialogueController.ShowDialogueFinish();
            return;
        }

        // 如果还没拿走娃娃，优先拿走娃娃
        if (!InventoryManager.Instance.HasItem(ItemName.Doll))
        {
            TakeDoll();
            return;
        }

        if (itemName == ItemName.Glass)
        {
            GiveGlassAndFinish();
        }
        else
        {
            dialogueController.ShowDialogueEmpty();
        }
    }

    private void TakeDoll()
    {
        // 【移除】不再设置 hasDoll = false;
        
        if (emptyHandedSprite != null)
        {
            spriteRenderer.sprite = emptyHandedSprite;
        }

        // 娃娃进背包
        InventoryManager.Instance.AddItem(ItemName.Doll);
        
        // 注意：AddItem 内部可能会触发 UI 刷新，但不会触发 Talbert 的图片刷新
        // 所以这里手动设置一下 sprite 确保即时反馈
    }

    private void GiveGlassAndFinish()
    {
        EventHandler.CallItemUsedEvent(ItemName.Glass);

        spriteRenderer.sprite = openSprite;
        EventHandler.CallItemGivenToNPCEvent(ItemName.Glass);

        isDone = true;
        dialogueController.ShowDialogueFinish();
    }

    private void ShowHint(string message)
    {
        var toastMgr = FindObjectOfType<ToastManager>();
        if (toastMgr != null) toastMgr.ShowToast(message);
        else Debug.Log($"[Hint] {message}");
    }
}