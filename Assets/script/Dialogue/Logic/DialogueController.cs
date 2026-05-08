using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueData_SO dialogueEmpty;
    public DialogueData_SO dialogueFinish;

    private Stack<string> dialogueEmptyStack;
    private Stack<string> dialogueFinishStack;
    private bool isTalking;

    // 【新增】用于存储当前正在运行的对话协程，以便随时打断
    private Coroutine currentDialogueCoroutine;

    private void Awake()
    {
        FillDialogueStack();
    }

    private void FillDialogueStack()
    {
        if (dialogueEmpty != null)
        {
            dialogueEmptyStack = new Stack<string>();
            for (int i = dialogueEmpty.dialogueList.Count - 1; i > -1; i--)
            {
                dialogueEmptyStack.Push(dialogueEmpty.dialogueList[i]);
            }
        }

        if (dialogueFinish != null)
        {
            dialogueFinishStack = new Stack<string>();
            for (int i = dialogueFinish.dialogueList.Count - 1; i > -1; i--)
            {
                dialogueFinishStack.Push(dialogueFinish.dialogueList[i]);
            }
        }
    }

    /// <summary>
    /// 【新增】私有方法：统一处理打断旧对话并启动新对话的逻辑
    /// </summary>
    private void StartNewDialogue(Stack<string> data)
    {
        if (data == null || data.Count == 0) return;

        // 1. 如果之前有对话正在播放，强制停止它
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            currentDialogueCoroutine = null;
        }

        // 2. 启动新的对话协程，并保存引用
        currentDialogueCoroutine = StartCoroutine(DialogueRoutine(data));
    }

    public void ShowDialogueEmpty()
    {
        if (dialogueEmptyStack != null)
        {
            // 使用新的统一方法
            StartNewDialogue(new Stack<string>(dialogueEmptyStack));
        }
    }

    public void ShowDialogueFinish()
    {
        if (dialogueFinishStack != null)
        {
            // 使用新的统一方法
            StartNewDialogue(new Stack<string>(dialogueFinishStack));
        }
    }

    /// <summary>
    /// 动态显示一组对话数据
    /// </summary>
    public void ShowDynamicDialogue(DialogueData_SO data)
    {
        if (data == null || data.dialogueList.Count == 0) return;

        Stack<string> dynamicStack = new Stack<string>();
        for (int i = data.dialogueList.Count - 1; i > -1; i--)
        {
            dynamicStack.Push(data.dialogueList[i]);
        }
        
        // 使用新的统一方法，这会自动打断之前的 Cliff 对话并显示 Bully 对话
        StartNewDialogue(dynamicStack);
    }

    private IEnumerator DialogueRoutine(Stack<string> data)
    {
        isTalking = true;
        
        while (data.Count > 0)
        {
            if (data.TryPop(out string result))
            {
                EventHandler.CallShowDialogueEvent(result);
                
                // 暂停让玩家看清
                yield return new WaitForSeconds(6f); 
            }
        }

        // 只有当自然结束（而不是被打断）时，才执行清理工作
        // 注意：如果被 StopCoroutine 打断，下面的代码不会执行
        EventHandler.CallShowDialogueEvent(string.Empty);
        isTalking = false;
        EventHandler.CallGameStateChangeEvent(GameState.GamePlay);
        
        // 清空当前协程引用
        currentDialogueCoroutine = null;
    }
}