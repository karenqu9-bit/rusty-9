using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // 任务状态标记
    public bool isCliffDone { get; private set; } = false;
    public bool isTalbertRoutineDone { get; set; } = false;
    // 在 QuestManager.cs 中添加

    // 【新增】电话任务完成标记
    public bool isPhoneDone { get; private set; } = false;

    // 新增公开方法，供 TalbertS1 调用

    // 【新增】电话打通方法
    public void SetPhoneDone()
    {
        isPhoneDone = true;
    }

    public void SetTalbertRoutineDone()
    {
        isTalbertRoutineDone = true;
    }
    public bool isAmandaDone { get; private set; } = false;
    public bool isGemmaPermanentlyGone { get; private set; } = false;

    public bool isTalbertDone { get; private set; } = false;
    public bool isGemmaGone { get; private set; } = false;

    // 在 QuestManager.cs 中添加
    public bool isDollTaken { get; private set; } = false;

    // 新增公开方法，供 TalbertS1 调用
    public void SetDollTaken()
    {
        isDollTaken = true;
    }

    private void Awake()
    {
        // 单例模式标准写法
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 确保场景切换后不销毁
    }

    private void OnEnable()
    {
        // 【关键】注册监听事件
        EventHandler.OnItemGivenToNPCEvent += HandleItemGiven;
    }

    private void OnDisable()
    {
        // 【关键】注销监听，防止内存泄漏
        EventHandler.OnItemGivenToNPCEvent -= HandleItemGiven;
    }

    // 处理接收到的物品事件
    private void HandleItemGiven(ItemName itemName)
    {
        Debug.Log($"[QuestManager] Received item: {itemName}");

        switch (itemName)
        {
            case ItemName.Bear:
                isCliffDone = true;
                break;
            case ItemName.Mask:
                isAmandaDone = true;
                // 【删除】 EventHandler.CallRewardEvent(ItemName.EmptyCase); 
                // 现在不需要在这里触发任何事件，UnlockableItem 脚本会自己监听 OnItemGivenToNPCEvent
                break;
            case ItemName.FullCase:
                isTalbertDone = true;
                break;

        }

        // 可选：如果所有任务都完成了，可以立即触发某些全局效果
        if (isCliffDone && isAmandaDone && isTalbertDone)
        {
            Debug.Log("All main tasks completed!");
            // EventHandler.CallAllTasksCompletedEvent(); // 如果需要可以再加一个全完成事件
        }
    }

    // 【新增】公开方法，供 Gemma 脚本调用
    public void SetGemmaGone()
    {
        isGemmaGone = true;
        isGemmaPermanentlyGone = true; // 永久消失
        Debug.Log("[QuestManager] Gemma has disappeared.");
    }


    public bool AreAllTasksCompleted()
    {
        return isCliffDone && isAmandaDone && isTalbertDone;
    }
    // 【新增】检查是否进入最终阶段
    public bool IsFinalStageReached()
    {
        return AreAllTasksCompleted() && isGemmaGone;
    }
}