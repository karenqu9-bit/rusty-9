using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4FinalTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject gemmaObject; // 在 Inspector 中拖入 S4 场景里的 Gemma 物体

    private void OnMouseDown()
    {
        // 1. 检查前置条件：必须完成前三个任务
        if (QuestManager.Instance == null || !QuestManager.Instance.AreAllTasksCompleted())
        {
            Debug.Log("[S4Trigger] Tasks not completed yet.");
            return;
        }

        // 2. 检查是否已经触发过（避免重复执行）
        if (QuestManager.Instance.IsFinalStageReached())
        {
            return; 
        }

        TriggerFinalSequence();
    }

    private void TriggerFinalSequence()
    {
        Debug.Log("[S4Trigger] Final sequence triggered!");

        // 3. 更新 QuestManager 状态
        QuestManager.Instance.SetGemmaGone();

        // 4. 让 Gemma 消失
        if (gemmaObject != null)
        {
            gemmaObject.SetActive(false);
        }
        else
        {
            // 如果没在 Inspector 赋值，尝试自动查找
            GameObject foundGemma = GameObject.Find("Gemma"); // 假设 Gemma 物体名字叫 "Gemma"
            if (foundGemma != null) foundGemma.SetActive(false);
        }

        // 5. 切换背景到 Final BG
        S4BackgroundController bgController = FindObjectOfType<S4BackgroundController>();
        if (bgController != null)
        {
            bgController.UpdateBackground();
        }
    }
}