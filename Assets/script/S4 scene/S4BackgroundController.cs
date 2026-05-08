using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4BackgroundController : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public Sprite normalBg;
    public Sprite specialBg;
    public Sprite finalBg;

    private void Start()
    {
        UpdateBackground();
    }

    private void OnEnable()
    {
        // 监听全完成事件（可选，如果想实时切换）
        // 或者直接在 Start 里检查 QuestManager 的状态
        UpdateBackground();
    }

    public void UpdateBackground()
    {
        if (backgroundRenderer == null) return;

        // 【关键】直接查询 QuestManager 的状态
        if (QuestManager.Instance == null) return;

        // 【逻辑优先级】
        // 1. 最高优先级：Gemma 消失了 -> 显示最终背景
        if (QuestManager.Instance.IsFinalStageReached())
        {
            backgroundRenderer.sprite = finalBg;
        }
        // 2. 次级优先级：三个任务完成了 -> 显示特殊背景
        else if (QuestManager.Instance.AreAllTasksCompleted())
        {
            backgroundRenderer.sprite = specialBg;
        }
        // 3. 默认：显示普通背景
        else
        {
            backgroundRenderer.sprite = normalBg;
        }
    }
}