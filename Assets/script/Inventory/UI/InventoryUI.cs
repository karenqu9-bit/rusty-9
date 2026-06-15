using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryContainer; // 拖入 InventoryContainer
    public GameObject slotPrefab;       // 拖入 SlotPrefab

    private List<SlotUI> activeSlots = new List<SlotUI>();

    private void OnEnable()
    {
        // 监听新的刷新事件
        EventHandler.RefreshInventoryEvent += OnRefreshInventoryEvent;
        
        // 初始刷新
        OnRefreshInventoryEvent(InventoryManager.Instance.GetItemList()); // 需要在 Manager 加个 getter 或直接访问
    }

    private void OnDisable()
    {
        EventHandler.RefreshInventoryEvent -= OnRefreshInventoryEvent;
    }

    // 为了方便访问，建议在 InventoryManager 加一个 public List<ItemName> GetItemList() { return itemList; }
    // 或者这里直接通过 Instance 访问私有字段（如果允许）
    
    private void OnRefreshInventoryEvent(List<ItemName> itemList)
    {
        // 1. 清除旧的槽位
        foreach (var slot in activeSlots)
        {
            Destroy(slot.gameObject);
        }
        activeSlots.Clear();

        // 2. 根据当前物品列表生成新槽位
        if (itemList == null || itemList.Count == 0)
        {
            // 可选：显示空背包提示
            return;
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            // 实例化槽位
            GameObject newSlotObj = Instantiate(slotPrefab, inventoryContainer);
            SlotUI newSlot = newSlotObj.GetComponent<SlotUI>();
            
            // 获取物品详情
            ItemName itemName = itemList[i];
            ItemDetails details = InventoryManager.Instance.GetItemDetails(itemName);
            
            // 设置槽位数据
            if (details != null)
            {
                newSlot.SetItem(details);
            }
            else
            {
                newSlot.SetEmpty();
            }

            activeSlots.Add(newSlot);
        }
    }
}