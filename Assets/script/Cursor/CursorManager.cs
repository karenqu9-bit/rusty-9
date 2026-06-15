using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public RectTransform hand;
    private Vector3 mouseWorldPos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

    private ItemName currentItem;
    private bool canClick;

    private bool holdItem;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
    }



    private void Update()
    {
        canClick = ObjectAtMousePosition();

        if (hand.gameObject.activeInHierarchy)
            hand.position = Input.mousePosition;

        if (canClick && Input.GetMouseButtonDown(0))
        {
            //检测鼠标互动情况
            ClickAction(ObjectAtMousePosition().gameObject);
        }
    }

    private void OnItemUsedEvent(ItemName obj)
    {
        currentItem = ItemName.None;
        holdItem = false;
        hand.gameObject.SetActive(false);
    }

    // Assets/script/Cursor/CursorManager.cs

    // 在 Update 或 OnItemSelectedEvent 中确保状态同步
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        holdItem = isSelected;
        if (isSelected)
        {
            // 只有当选中和细节都不为空时才更新
            if (itemDetails != null)
            {
                currentItem = itemDetails.itemName;
            }
        }
        else
        {
            currentItem = ItemName.None;
        }

        // 【关键修改】不仅依赖 isSelected，还要检查 InventoryManager 是否真的还有这个物品
        // 防止合成后 UI 还没刷新但 Cursor 还拿着图的情况
        if (holdItem && currentItem != ItemName.None && !InventoryManager.Instance.HasItem(currentItem))
        {
            holdItem = false;
            currentItem = ItemName.None;
        }

        hand.gameObject.SetActive(holdItem);
    }

    private void ClickAction(GameObject clickObject)
    {
        switch (clickObject.tag)
        {
            case "Teleport":
                var teleport = clickObject.GetComponent<Teleport>();
                teleport?.TeleportToScene();
                break;
            case "Item":
                var item = clickObject.GetComponent<Item>();
                item?.ItemClicked();
                break;
            case "Interactive":
                var interactive = clickObject.GetComponent<Interactive>();
                if (holdItem)
                    interactive?.CheckItem(currentItem);
                else
                    interactive?.EmptyClicked();

                break;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Collider2D ObjectAtMousePosition()
    {
        return Physics2D.OverlapPoint(mouseWorldPos);
    }
}
