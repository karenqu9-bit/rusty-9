using System;
using System.Collections.Generic;
using UnityEngine;


public static class EventHandler
{
    // 声明通用解锁事件（带物体名称参数）
    public static event Action<string> OnGenericUnlockEvent;
    // 触发通用解锁事件的方法
    public static void CallGenericUnlockEvent(string objectName)
    {
        OnGenericUnlockEvent?.Invoke(objectName);
    }
    // 声明通用解锁事件

    // 声明Talbert协程完成事件
    public static event Action OnTalbertRoutineDoneEvent;
    // 触发Talbert协程完成事件的方法
    public static void CallTalbertRoutineDoneEvent()
    {
        OnTalbertRoutineDoneEvent?.Invoke();
    }


    // 在 EventHandler 中添加
    public static event System.Action<List<ItemName>> RefreshInventoryEvent;

    public static void CallRefreshInventoryEvent(List<ItemName> itemList)
    {
        if (RefreshInventoryEvent != null)
            RefreshInventoryEvent(itemList);
    }
    public static event Action<ItemDetails, int> UpdateUIEvent;
    public static void CallUpdateUIEvent(ItemDetails itemDetails, int index)
    {
        UpdateUIEvent?.Invoke(itemDetails, index);
    }
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;

    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<ItemName> ItemUsedEvent;
    public static void CallItemUsedEvent(ItemName itemName)
    {
        ItemUsedEvent?.Invoke(itemName);
    }
    public static event Action<int> ChangeItemEvent;
    public static void CallChangeItemEvent(int index)
    {
        ChangeItemEvent?.Invoke(index);
    }

    public static event Action<string> ShowDialogueEvent;
    public static void CallShowDialogueEvent(string dialogue)
    {
        ShowDialogueEvent?.Invoke(dialogue);
    }
    public static event Action<GameState> GameStateChangeEvent;
    public static void CallGameStateChangeEvent(GameState gameState)
    {
        GameStateChangeEvent?.Invoke(gameState);
    }
    // 【新增】当玩家向 NPC 给予物品时触发
    // 参数：给予的物品名称
    public static event Action<ItemName> OnItemGivenToNPCEvent;

    public static void CallItemGivenToNPCEvent(ItemName itemName)
    {
        OnItemGivenToNPCEvent?.Invoke(itemName);
    }
}

