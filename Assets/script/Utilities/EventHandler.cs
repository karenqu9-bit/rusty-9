using System;
using UnityEngine;

public static class EventHandler
{
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