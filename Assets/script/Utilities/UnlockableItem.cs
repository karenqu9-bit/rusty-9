using UnityEngine;

public class UnlockableItem : MonoBehaviour
{
    [Header("Unlock Condition")]
    public ItemName requiredItemToUnlock;

    [Header("Settings")]
    public bool startHidden = true;

    // 【新增】标记是否已经解锁
    private bool isUnlocked = false;

    private void Awake()
    {
        // 1. 注册事件
        EventHandler.OnItemGivenToNPCEvent += CheckUnlockCondition;
        EventHandler.OnGenericUnlockEvent += OnGenericUnlock; // 监听通用解锁事件

        // 2. 初始隐藏
        if (startHidden && !isUnlocked)
        {
            gameObject.SetActive(false);
        }
    }



    private void OnDestroy()
    {
        EventHandler.OnItemGivenToNPCEvent -= CheckUnlockCondition;
        EventHandler.OnGenericUnlockEvent -= OnGenericUnlock; ; // 取消监听通用解锁事件
    }

    private void CheckUnlockCondition(ItemName givenItem)
    {
        if (isUnlocked) return;

        if (givenItem == requiredItemToUnlock)
        {
            UnlockItem();
        }
    }

    private void UnlockItem()
    {
        isUnlocked = true; // 先标记为已解锁
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log($"[UnlockableItem] Activating {gameObject.name}!");
            gameObject.SetActive(true);
        }
    }

    // 【新增】公开方法：供 ObjectManager 调用以恢复状态
    public void SetUnlockState(bool state)
    {
        isUnlocked = state;
        if (state)
        {
            // 必须在这里强制激活，因为 Item 的恢复逻辑已经不管它了
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (startHidden && gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        }


    }

    // 【新增】公开方法：供 ObjectManager 查询当前状态
    public bool GetUnlockState()
    {
        return isUnlocked;
    }
    // 通用解锁响应：不校验物品，直接激活

    private void OnGenericUnlock(string objectName)
    {
        if (isUnlocked) return;
        // 只有当传入的名称与自身名称一致时才解锁
        if (gameObject.name == objectName)
        {
            UnlockItem();
        }
    }

}