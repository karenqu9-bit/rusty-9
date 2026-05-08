using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Button leftButton, rightButton;
    public SlotUI slotUI;
    
    // 本地缓存当前显示的索引，用于快速判断
    private int currentIndex = -1; 

    private void OnEnable()
    {
        EventHandler.UpdateUIEvent += OnUpdateUIEvent;
        // 初始化按钮状态
        UpdateButtonInteractability();
    }

    private void OnDisable()
    {
        EventHandler.UpdateUIEvent -= OnUpdateUIEvent;
    }

    private void OnUpdateUIEvent(ItemDetails itemDetails, int index)
    {
        if (itemDetails == null)
        {
            slotUI.SetEmpty();
            currentIndex = -1;
        }
        else
        {
            // 只有当索引匹配时才更新显示内容？
            // 注意：CallUpdateUIEvent 可能被用来刷新特定槽位，也可能用来切换选中项
            // 在 InventoryManager 中，CallChangeItemEvent 会触发 CallUpdateUIEvent(item, index)
            // 所以这里的 index 就是当前选中的索引
            
            currentIndex = index;
            slotUI.SetItem(itemDetails);
        }
        
        // 【关键】每次 UI 数据更新后，重新计算按钮的可交互状态
        UpdateButtonInteractability();
    }

    /// <summary>
    /// 根据当前索引和背包总数量，更新左右按钮的状态
    /// </summary>
    private void UpdateButtonInteractability()
    {
        int totalCount = InventoryManager.Instance.GetItemCount();

        if (totalCount <= 0)
        {
            leftButton.interactable = false;
            rightButton.interactable = false;
            return;
        }

        // 左边按钮：只要当前不是第一个，就可以左移
        leftButton.interactable = (currentIndex > 0);

        // 右边按钮：只要当前不是最后一个，就可以右移
        rightButton.interactable = (currentIndex < totalCount - 1);
    }
    
    /// <summary>
    /// 左右按键点击事件
    /// </summary>
    /// <param name="amount">-1 为左，1 为右</param>
    public void SwitchItem(int amount)
    {
        int totalCount = InventoryManager.Instance.GetItemCount();
        if (totalCount <= 0) return;

        int nextIndex = currentIndex + amount;

        // 【关键修复】在发送事件前，进行严格的边界检查
        if (nextIndex >= 0 && nextIndex < totalCount)
        {
            // 只有索引有效时，才通知 Manager 切换
            EventHandler.CallChangeItemEvent(nextIndex);
        }
        else
        {
            // 可选：如果希望循环选择，可以在这里处理
            // if (nextIndex < 0) nextIndex = totalCount - 1;
            // if (nextIndex >= totalCount) nextIndex = 0;
            // EventHandler.CallChangeItemEvent(nextIndex);
            
            // 如果不循环，则什么都不做，按钮状态会在 UpdateButtonInteractability 中保持禁用
        }
    }
}