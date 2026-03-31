using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public ItemDataList_SO itemData;
    [SerializeField]private List<ItemName> itemList = new List<ItemName>();

    private void OnEnable()
    {
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemUsedEvent -= OnItemUsedEvent; 
    }
    private void OnItemUsedEvent(ItemName itemName)
    {
        var index = GetItemIndex(itemName);
        itemList.RemoveAt(index);
        //暂时实现单一使用物品效果
        if(itemList.Count==0)
            EventHandler.CallUpdateUIEvent(null, -1);
    }
    public void AddItem(ItemName itemName)
    {
        if(!itemList.Contains(itemName))
        {
            itemList.Add(itemName);
            //UI对应显示
            EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemName),itemList.Count - 1);
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
}
