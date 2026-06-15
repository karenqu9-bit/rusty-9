using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowtuziInteraction : Interactive
{
    private ItemName lastSelectedItem = ItemName.None;

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
        Debug.Log("[Yellowtuzi] 空手点击了兔子");
    }

    public override void CheckItem(ItemName itemName)
    {
        // 实时查询全局状态，判断 Talbert 协程是否已完成
        bool isTalbertDone = QuestManager.Instance != null && QuestManager.Instance.isTalbertRoutineDone;

        // 只有Talbert协程完成，且手持兔子脚时才能触发交互
        if (isTalbertDone && lastSelectedItem == ItemName.tuzifoot)
        {
            // 触发给予物品事件，参数为 tuzifoot
            EventHandler.CallItemGivenToNPCEvent(ItemName.tuzifoot);
            // 消耗掉兔子脚
            EventHandler.CallItemUsedEvent(ItemName.tuzifoot);
        }
        else
        {
            if (!isTalbertDone)
            {
                Debug.Log("[Yellowtuzi] Talbert协程尚未完成，无法交互");
            }
            else
            {
                Debug.Log($"[Yellowtuzi] 不能使用 {lastSelectedItem} 交互");
            }
        }
    }
}
