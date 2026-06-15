using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryManager : Singleton<InventoryManager>
{
    public ItemDataList_SO itemData;
    [SerializeField] private List<ItemName> itemList = new List<ItemName>();

    private ItemName currentHeldItem = ItemName.None;
    private bool isProcessing = false;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }




    private void OnAfterSceneLoadedEvent()
    {
        // 场景加载后，刷新整个背包UI
        RefreshAllUI();
    }

    // 【重构】刷新所有UI：通知UI层重新渲染所有槽位
    public void RefreshAllUI()
    {
        // 调用 UI 层的刷新方法，传入整个列表
        // 假设我们在 InventoryUI 中增加了一个新方法 CallRefreshInventory
        EventHandler.CallRefreshInventoryEvent(itemList);
    }

    // 【核心重构】处理物品选中逻辑
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // 1. 如果正在处理切换逻辑，直接忽略所有传入的事件（防止递归）
        if (isProcessing) return;

        if (isSelected && itemDetails != null)
        {
            ItemName clickedItemName = itemDetails.itemName;

            // 情况1：当前手里没东西 -> 拿起
            if (currentHeldItem == ItemName.None)
            {
                currentHeldItem = clickedItemName;
                Debug.Log($"[Manager] Picked up: {clickedItemName}");
            }
            // 情况2：手里有东西，且点击的是同一个 -> 放下
            else if (currentHeldItem == clickedItemName)
            {
                currentHeldItem = ItemName.None;
                Debug.Log($"[Manager] Put down: {clickedItemName}");
            }
            // 情况3：手里有东西，且点击了不同的物品 -> 尝试切换/合成
            else
            {
                ItemName oldHeldItem = currentHeldItem;
                Debug.Log($"[Manager] Switching from {oldHeldItem} to {clickedItemName}");

                // --- 开始原子操作 ---
                isProcessing = true;

                // 【关键修复】在切换前，先强制通知所有 UI “放下” 当前手持的物品
                // 这能确保 SlotUI 里的 isSelected 被重置为 false
                ItemDetails oldDetails = GetItemDetails(oldHeldItem);
                if (oldDetails != null)
                {
                    EventHandler.CallItemSelectedEvent(oldDetails, false);
                }

                // 1. 更新内部状态为新物品
                currentHeldItem = clickedItemName;

                // 2. 判断是否合成
                if (TryCombineItems(currentHeldItem, oldHeldItem))
                {
                    Debug.Log("[Manager] Combine Success!");

                    // 合成成功：清空手
                    currentHeldItem = ItemName.None;

                    // 刷新背包列表（这会重建所有 SlotUI，彻底清除残留状态）


                    // 确保 Cursor 隐藏
                    EventHandler.CallItemSelectedEvent(null, false);
                    HideTooltipIfEmpty();

                    RefreshAllUI();
                    StartCoroutine(ForceHideTooltipNextFrame());
                }
                else
                {
                    Debug.Log($"[Manager] Combine Failed. Keeping {clickedItemName} in hand.");

                    // 合成失败：现在旧物品已经被上面的代码“放下”了
                    // 我们只需要通知新物品“被拿起”
                    // 注意：这里触发 true 事件，SlotUI 会把自己的 isSelected 设为 true
                    EventHandler.CallItemSelectedEvent(itemDetails, true);
                }

                // --- 结束原子操作 ---
                isProcessing = false;

                return;
            }
        }
        else if (!isSelected)
        {
            // 处理放下的逻辑
            if (itemDetails != null && itemDetails.itemName == currentHeldItem)
            {
                currentHeldItem = ItemName.None;
            }
        }
    }

    private System.Collections.IEnumerator ForceHideTooltipNextFrame()
    {
        yield return null; // 等待一帧，让 InventoryUI 完成实例化和布局
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.gameObject.SetActive(false);
        }

        // 如果还不放心，可以再等一帧确认
        yield return null;
        if (ItemTooltip.Instance != null)
        {
            ItemTooltip.Instance.gameObject.SetActive(false);
        }
    }

    private void OnItemUsedEvent(ItemName itemName)
    {
        Debug.Log($"[Manager] Item Used Event Received: {itemName}");

        // 如果使用的是当前手持的物品，也要清空手持状态
        if (currentHeldItem == itemName)
        {
            currentHeldItem = ItemName.None;
            HideTooltipIfEmpty();
        }

        // 从背包中移除该物品
        RemoveItemByName(itemName);
    }

    /// <summary>
    /// 尝试将 heldItem 使用在 targetItem 上
    /// 返回 true 表示成功消耗并改变了物品，false 表示无操作
    /// </summary>
    /// 
    private bool TryCombineItems(ItemName newItem, ItemName oldItem)
    {
        // 配方：Glass + EmptyCase -> FullCase
        bool isMatch = (newItem == ItemName.Glass && oldItem == ItemName.EmptyCase) ||
                       (newItem == ItemName.EmptyCase && oldItem == ItemName.Glass);

        if (isMatch)
        {
            // 1. 移除旧物品
            int index1 = GetItemIndex(newItem);
            if (index1 != -1) itemList.RemoveAt(index1);

            int index2 = GetItemIndex(oldItem);
            if (index2 != -1) itemList.RemoveAt(index2);

            // 2. 添加新物品
            if (!itemList.Contains(ItemName.FullCase))
            {
                itemList.Add(ItemName.FullCase);
            }

            return true;
        }

        return false;
    }


    public void RemoveItemByName(ItemName itemName)
    {
        var index = GetItemIndex(itemName);
        if (index != -1)
        {
            itemList.RemoveAt(index);
            RefreshAllUI(); // 移除后刷新列表
        }
    }

    public void AddItem(ItemName itemName)
    {
        if (!itemList.Contains(itemName))
        {
            itemList.Add(itemName);
            RefreshAllUI(); // 添加后刷新列表
            Debug.Log($"[Manager] Item Added: {itemName}");
        }
        else
        {
            Debug.Log($"[Manager] Item {itemName} already exists.");
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

    // 辅助方法：获取物品详情
    // 【新增】提供公共接口供 UI 层获取当前物品列表
    public List<ItemName> GetItemList()
    {
        return itemList;
    }

    // 辅助方法：获取物品详情
    public ItemDetails GetItemDetails(ItemName itemName)
    {
        return itemData.GetItemDetails(itemName);
    }

    // 【新增】获取当前手持物品，供 CursorManager 或其他系统查询
    public ItemName GetCurrentHeldItem()
    {
        return currentHeldItem;
    }
    private void HideTooltipIfEmpty()
    {
        if (currentHeldItem == ItemName.None)
        {
            if (ItemTooltip.Instance != null)
            {
                ItemTooltip.Instance.gameObject.SetActive(false);
            }
        }
    }
}