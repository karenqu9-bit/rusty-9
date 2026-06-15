using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;
    // 【删除】不再需要 public ItemTooltip tooltip;

    private ItemDetails currentItem;
    private bool isSelected = false;

    private void Awake()
    {
        // 【删除】不再需要查找 Tooltip，直接使用单例
    }
    private void OnEnable()
    {
        // 【新增】监听全局选中事件，以便同步状态
        EventHandler.ItemSelectedEvent += OnGlobalItemSelectedEvent;
    }

    private void OnDisable()
    {
        // 【新增】取消监听
        EventHandler.ItemSelectedEvent -= OnGlobalItemSelectedEvent;
    }

    // 【新增】当其他地方（如 InventoryManager）触发选中/取消事件时，同步此槽位的状态
    private void OnGlobalItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // 如果事件是关于当前槽位的物品
        if (currentItem != null && itemDetails != null && currentItem.itemName == itemDetails.itemName)
        {
            this.isSelected = isSelected;

            // 同步 Tooltip 显示
            ItemTooltip globalTooltip = ItemTooltip.Instance;
            if (globalTooltip != null)
            {
                if (isSelected)
                {
                    globalTooltip.UpdateItemName(currentItem.itemName);
                    globalTooltip.gameObject.SetActive(true);
                }
                else
                {
                    // 只有当当前槽位是最后一个被选中的，才隐藏 Tooltip
                    // 这里简化处理：如果取消选中，且当前槽位就是那个物品，则隐藏
                    // 更严谨的做法是检查 InventoryManager 当前手持的是什么
                    if (InventoryManager.Instance.GetCurrentHeldItem() == ItemName.None)
                    {
                        globalTooltip.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    public void SetItem(ItemDetails itemDetails)
    {
        currentItem = itemDetails;
        this.gameObject.SetActive(true);

        // 重置选中状态
        isSelected = false;

        // 【安全检查】防止 itemImage 未赋值
        if (itemImage == null)
        {
            Debug.LogError("[SlotUI] Item Image is not assigned in the Prefab!");
            return;
        }

        if (itemDetails != null && itemDetails.itemSprite != null)
        {
            itemImage.sprite = itemDetails.itemSprite;
        }
        else
        {
            itemImage.sprite = null;
        }
    }

    public void SetEmpty()
    {
        currentItem = null;
        isSelected = false;
        this.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            // 获取全局唯一的 Tooltip 实例
            ItemTooltip globalTooltip = ItemTooltip.Instance;

            if (globalTooltip == null)
            {
                Debug.LogError("[SlotUI] ItemTooltip Instance not found! Make sure ItemTooltip script is on an active GameObject in the scene.");
                return;
            }

            // 【逻辑修改】切换选中状态
            isSelected = !isSelected;

            if (isSelected)
            {
                // --- 玩家拿取物品 ---

                // 1. 更新并显示全局物品名称
                globalTooltip.UpdateItemName(currentItem.itemName);
                globalTooltip.gameObject.SetActive(true);

                // 2. 触发选中事件
                EventHandler.CallItemSelectedEvent(currentItem, true);
                Debug.Log($"[SlotUI] Item PICKED UP: {currentItem.itemName}");
            }
            else
            {
                // --- 玩家放下物品 ---

                // 1. 隐藏全局物品名称
                globalTooltip.gameObject.SetActive(false);

                // 2. 触发取消选中事件
                EventHandler.CallItemSelectedEvent(currentItem, false);
                Debug.Log($"[SlotUI] Item PUT DOWN: {currentItem.itemName}");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 悬停不做任何事
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 移出不做任何事
    }
}