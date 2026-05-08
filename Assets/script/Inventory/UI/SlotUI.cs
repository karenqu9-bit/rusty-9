using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;
    public ItemTooltip tooltip;
    
    private ItemDetails currentItem;
    private bool isSelected = false; // 本地记录选中状态，用于视觉反馈

    public void SetItem(ItemDetails itemDetails)
    {
        currentItem = itemDetails;
        this.gameObject.SetActive(true);
        if (itemDetails != null && itemDetails.itemSprite != null)
        {
            itemImage.sprite = itemDetails.itemSprite;
            itemImage.SetNativeSize();
        }
        // 重置选中视觉效果
        isSelected = false;
        UpdateVisualSelection();
    }

    public void SetEmpty()
    {
        currentItem = null;
        this.gameObject.SetActive(false);
        isSelected = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            isSelected = !isSelected; // 切换选中状态
            
            // 【关键】发送选中事件，让 Manager 决定要不要删掉上一个选中的
            EventHandler.CallItemSelectedEvent(currentItem, isSelected);
            
            UpdateVisualSelection();
            Debug.Log($"[SlotUI] Toggled Selection for {currentItem.itemName}: {isSelected}");
        }
    }

    // 简单的视觉反馈：选中时变亮，未选中时正常
    private void UpdateVisualSelection()
    {
        if (itemImage != null)
        {
            Color c = itemImage.color;
            c.a = isSelected ? 1.0f : 0.8f; // 示例：选中时完全不透明，未选中时稍微透明
            itemImage.color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.gameObject.activeInHierarchy && currentItem != null)
        {
            tooltip.gameObject.SetActive(true);
            tooltip.UpdateItemName(currentItem.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(false);
    }
}