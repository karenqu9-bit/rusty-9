using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4FinalTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject gemmaObject; // 在 Inspector 中拖入 S4 场景里的 Gemma 物体

    private void OnMouseDown()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogError("[S4Trigger] QuestManager Instance is null!");
            return;
        }

        if (!QuestManager.Instance.AreAllTasksCompleted())
        {
            Debug.Log($"[S4Trigger] Tasks not completed yet. Cliff: {QuestManager.Instance.isCliffDone}, Amanda: {QuestManager.Instance.isAmandaDone}, Talbert: {QuestManager.Instance.isTalbertDone}");
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
            GameObject foundGemma = GameObject.Find("Gemma");
            if (foundGemma != null)
            {
                foundGemma.SetActive(false);
            }
        }

        // 5. 切换背景到 Final BG
        S4BackgroundController bgController = FindObjectOfType<S4BackgroundController>();
        if (bgController != null)
        {
            bgController.UpdateBackground();
        }
    }

}


