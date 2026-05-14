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
    
    // 【新增】私有 AudioSource 引用
    private AudioSource audioSource;

    private void Awake()
    {
        FillDialogueStack();
        
        // 【关键】确保当前 NPC 物体上有 AudioSource，如果没有则自动添加
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; // 2D 声音
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
    /// 【修改】统一处理打断旧对话并启动新对话的逻辑
    /// </summary>
    /// <param name="sourceData">原始数据对象，用于获取音效</param>
    /// <param name="stack">复制后的字符串栈，用于对话流程</param>
    private void StartNewDialogue(DialogueData_SO sourceData, Stack<string> stack)
    {
        if (stack == null || stack.Count == 0) return;

        // 1. 如果之前有对话正在播放，强制停止它
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            currentDialogueCoroutine = null;
        }

        // 【新增】播放该段对话专属的音效
        PlayDialogueSound(sourceData);

        // 2. 启动新的对话协程，并保存引用
        currentDialogueCoroutine = StartCoroutine(DialogueRoutine(stack));
    }

    // 【新增】播放音效辅助方法 (只保留一个！)
    private void PlayDialogueSound(DialogueData_SO data)
    {
        // 只有当数据不为空、音效不为空、且音频源存在时才播放
        if (data != null && data.dialogueSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(data.dialogueSound);
        }
    }

    public void ShowDialogueEmpty()
    {
        if (dialogueEmptyStack != null && dialogueEmptyStack.Count > 0)
        {
            // 【修复】必须传入 dialogueEmpty 作为第一个参数，以便获取音效
            StartNewDialogue(dialogueEmpty, new Stack<string>(dialogueEmptyStack));
        }
    }

    public void ShowDialogueFinish()
    {
        if (dialogueFinishStack != null && dialogueFinishStack.Count > 0)
        {
            // 【修复】必须传入 dialogueFinish 作为第一个参数，以便获取音效
            StartNewDialogue(dialogueFinish, new Stack<string>(dialogueFinishStack));
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

        // 使用新的统一方法，传入 data 以便播放对应音效
        StartNewDialogue(data, dynamicStack);
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