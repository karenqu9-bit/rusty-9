using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // 任务状态标记
    public bool isCliffDone { get; private set; } = false;
    public bool isAmandaDone { get; private set; } = false;
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
                break;
            case ItemName.Glass:
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
        Debug.Log("[QuestManager] Gemma has disappeared.");

        // 如果需要，这里也可以触发一个事件通知其他物体
        // EventHandler.CallGemmaDisappearedEvent(); 
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