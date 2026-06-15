using UnityEngine;

public class KeyHoleInteraction : Interactive
{
    [Header("Settings")]
    public int holeNumber; // 1 或 2，表示是第几个钥匙孔
    public ItemName correctItem; // 正确的物品名称

    private bool isFilled = false; // 是否已放置物品

    public override void EmptyClicked()
    {
        Debug.Log($"[KeyHole] EmptyClicked called on {gameObject.name} (Hole {holeNumber})!"); // 添加这行

        if (isFilled)
        {
            Debug.Log("[KeyHole] This hole is already filled!");
            return;
        }

        // 检查当前选中的物品
        ItemName selectedItem = InventoryManager.Instance.GetCurrentHeldItem();
        Debug.Log($"[KeyHole] Current selected item: {selectedItem}"); // 添加这行

        if (selectedItem == ItemName.None)
        {
            Debug.Log("[KeyHole] No item selected in inventory");
            return;
        }

        Debug.Log($"[KeyHole] Hole {holeNumber} expects: {correctItem}, but got: {selectedItem}"); // 添加这行

        // 检查是否是正确的物品
        if (selectedItem == correctItem)
        {
            Debug.Log("[KeyHole] Correct item! Placing item..."); // 添加这行
            // 放置物品
            PlaceItem(selectedItem);
        }
        else
        {
            Debug.Log($"[KeyHole] Wrong item! Expected {correctItem} but got {selectedItem}"); // 添加这行
        }
    }



    private void PlaceItem(ItemName item)
    {
        Debug.Log($"[KeyHole] Starting to place item: {item}"); // 添加这行

        isFilled = true;
        Debug.Log($"[KeyHole] Marked hole {holeNumber} as filled"); // 添加这行

        // 从背包中移除物品
        Debug.Log($"[KeyHole] Removing item {item} from inventory"); // 添加这行
        InventoryManager.Instance.RemoveItemByName(item);

        // 更新UI
        Debug.Log("[KeyHole] Calling UI update event"); // 添加这行
        EventHandler.CallUpdateUIEvent(new ItemDetails { itemName = item }, 0);

        // 检查是否完成解密
        Debug.Log("[KeyHole] Checking puzzle completion"); // 添加这行
        CheckPuzzleCompletion();
    }






    private void CheckPuzzleCompletion()
    {
        Debug.Log("[KeyHole] Checking puzzle completion...");

        // 查找所有钥匙孔
        KeyHoleInteraction[] keyHoles = FindObjectsOfType<KeyHoleInteraction>();
        Debug.Log($"[KeyHole] Found {keyHoles.Length} key holes");

        // 按照 holeNumber 排序
        System.Array.Sort(keyHoles, (a, b) => a.holeNumber.CompareTo(b.holeNumber));

        // 打印排序后的结果
        for (int i = 0; i < keyHoles.Length; i++)
        {
            Debug.Log($"[KeyHole] Sorted hole {i + 1}: number={keyHoles[i].holeNumber}, correctItem={keyHoles[i].correctItem}");
        }

        // 检查是否所有钥匙孔都已填充
        bool allFilled = true;
        foreach (var hole in keyHoles)
        {
            Debug.Log($"[KeyHole] Hole {hole.holeNumber} filled status: {hole.isFilled}");
            if (!hole.isFilled)
            {
                allFilled = false;
                break;
            }
        }

        Debug.Log($"[KeyHole] All holes filled: {allFilled}");

        // 检查填充顺序是否正确
        if (allFilled)
        {
            // 验证第一个和第二个钥匙孔是否填充了正确的物品
            bool isCorrectOrder = keyHoles[0].correctItem == ItemName.I && keyHoles[1].correctItem == ItemName.X;
            Debug.Log($"[KeyHole] Checking hole order: Hole 1 expects {keyHoles[0].correctItem}, Hole 2 expects {keyHoles[1].correctItem}");
            Debug.Log($"[KeyHole] Is correct order: {isCorrectOrder}");

            if (isCorrectOrder)
            {
                Debug.Log("[KeyHole] Puzzle completed successfully!");
                // 触发解密完成事件
                PuzzleCompletedEvent.CallPuzzleCompletedEvent();
            }
            else
            {
                Debug.Log("[KeyHole] Puzzle failed: Wrong item order!");
            }
        }
    }


    private void OnMouseDown()
    {
        Debug.Log("[KeyHole] OnMouseDown called!");
        EmptyClicked();
    }
}
