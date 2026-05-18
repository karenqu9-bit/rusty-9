using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemDataList_SO itemData;
    [SerializeField] private List<ItemName> itemList = new List<ItemName>();

    // 【核心状态】记录当前在背包UI中被“选中”的物品
    private ItemName selectedItemName = ItemName.None;

    private void OnEnable()
    {
        // 1. 监听背包点击：用于更新 selectedItemName
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;

        // 2. 监听交互/使用事件：用于触发移除逻辑
        // 无论是点击场景物体(Cliff)还是其他使用行为，都会触发这个
        EventHandler.ItemUsedEvent += OnItemUsedEvent;

        EventHandler.ChangeItemEvent += OnChangeItemEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
        EventHandler.ChangeItemEvent -= OnChangeItemEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 【修改】不再直接重置为 None
        // 检查之前选中的物品是否还在当前场景的背包列表中
        if (selectedItemName != ItemName.None && HasItem(selectedItemName))
        {
            // 如果还在，找到它的索引并重新触发 UI 更新，保持选中状态
            int index = GetItemIndex(selectedItemName);
            if (index != -1)
            {
                EventHandler.CallChangeItemEvent(index);
                Debug.Log($"[Manager] Scene loaded. Restoring selection: {selectedItemName}");
            }
        }
        else
        {
            // 如果之前选中的物品不在了（比如被消耗了或没带过来），则重置
            selectedItemName = ItemName.None;
            RefreshAllUI();
            Debug.Log("[Manager] Scene loaded. Selection reset because item is missing.");
        }
    }

    private void RefreshAllUI()
    {
        if (itemList.Count == 0)
        {
            EventHandler.CallUpdateUIEvent(null, -1);
        }
        else
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemList[i]), i);
            }
            if (itemList.Count > 0)
                EventHandler.CallChangeItemEvent(0);
        }
    }

    private void OnChangeItemEvent(int index)
    {
        if (itemList.Count == 0)
        {
            EventHandler.CallUpdateUIEvent(null, -1);
            return;
        }

        if (index >= 0 && index < itemList.Count)
        {
            ItemDetails item = itemData.GetItemDetails(itemList[index]);
            EventHandler.CallUpdateUIEvent(item, index);

            // 左右切换时，也更新选中状态，但不触发移除
            selectedItemName = itemList[index];
        }
    }

    // 【逻辑A】处理背包UI点击：只负责记录“谁被选中了”，不负责移除
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected || itemDetails == null)
        {
            selectedItemName = ItemName.None;
            Debug.Log("[Manager] Deselected item in UI.");
            return;
        }

        // 更新选中状态
        selectedItemName = itemDetails.itemName;
        Debug.Log($"[Manager] Item Selected in UI: {selectedItemName}. Waiting for interaction to use/remove it.");
    }

    // 【逻辑B】处理交互/使用：当发生交互时，检查是否有“选中物品”，如果有则移除
    private void OnItemUsedEvent(ItemName itemName)
    {
        Debug.Log($"[Manager] Interaction/Use Event Triggered by: {itemName}");

        // 【关键判断】
        // 如果当前有一个“被选中”的物品（比如小熊），且它还在背包里
        if (selectedItemName != ItemName.None && HasItem(selectedItemName))
        {
            Debug.Log($"[Manager] Removing previously selected item: {selectedItemName} because of interaction with {itemName}");

            // 移除选中的物品（小熊）
            RemoveItemByName(selectedItemName);

            // 重置选中状态，防止重复移除
            selectedItemName = ItemName.None;
        }
        else
        {
            Debug.Log("[Manager] No item was selected in inventory, or selected item no longer exists. Nothing removed from inventory.");
        }
    }

    private void RemoveItemByName(ItemName itemName)
    {
        var index = GetItemIndex(itemName);
        if (index != -1)
        {
            itemList.RemoveAt(index);
            RefreshAllUI();
        }
    }

    public void AddItem(ItemName itemName)
    {
        // 1. 检查是否已存在（根据你的游戏设计，如果需要堆叠请修改此逻辑）
        if (!itemList.Contains(itemName))
        {
            itemList.Add(itemName);

            // 2. 获取新加入物品的索引（即列表最后一个元素的索引）
            int newIndex = itemList.Count - 1;

            Debug.Log($"[Manager] Item Added: {itemName} at index {newIndex}");

            // 3. 【关键】直接更新 UI 显示这个新物品，并选中它
            // 这样就不会像 RefreshAllUI 那样重置到索引 0

            // A. 通知 UI 更新该槽位的数据（显示图片等）
            ItemDetails newitemDetails = itemData.GetItemDetails(itemName);
            EventHandler.CallUpdateUIEvent(newitemDetails, newIndex);

            /*
            // B. 通知 UI 切换选中项到这个新物品（高亮、更新按钮状态）
            EventHandler.CallChangeItemEvent(newIndex);
            */
            Debug.Log($"[Manager] Keeping current selection. New item added to slot {newIndex}.");
        }
        else
        {
            Debug.Log($"[Manager] Item {itemName} already exists.");
            // 如果允许堆叠，这里可以处理数量增加逻辑
        }
    }

    private int GetItemIndex(ItemName itemName)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == itemName)
                return i;
        }
        return -1;
    }

    public bool HasItem(ItemName itemName)
    {
        return itemList.Contains(itemName);
    }

    public int GetItemCount()
    {
        return itemList.Count;
    }
}