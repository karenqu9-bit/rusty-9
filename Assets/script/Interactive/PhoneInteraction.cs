using System.Collections.Generic;
using UnityEngine;

// 改回继承 Interactive，兼容 ObjectManager 存档
public class PhoneInteraction : Interactive
{
    [Header("Phone Settings")]
    [Tooltip("正确的按键顺序，例如 1,2,3,4")]
    public List<int> correctSequence = new List<int> { 1, 2, 3, 4 };

    private List<int> currentInput = new List<int>();
    private bool isUnlocked = false;

    public override void EmptyClicked()
    {
        if (isUnlocked) return;
        Debug.Log("[Phone] 请拨号...");
    }

    // 不需要 CheckItem，电话不使用物品交互
    public override void CheckItem(ItemName itemName)
    {
        // 必须重写并留空，否则基类可能会消费物品
    }

    // 【核心】供 12 个按键碰撞体调用的公开方法
    public void OnKeyPressed(int keyNumber)
    {
        if (isDone) return;

        currentInput.Add(keyNumber);
        string currentInputString = string.Join(", ", currentInput);
        Debug.Log($"<color=cyan>[Phone Debug]</color> 点击成功！按下按键: {keyNumber} | 当前已输入: [{currentInputString}]");

        if (currentInput.Count >= correctSequence.Count)
        {
            CheckSequence();
        }
    }

    private void CheckSequence()
    {
        bool isCorrect = true;
        for (int i = 0; i < correctSequence.Count; i++)
        {
            if (currentInput[i] != correctSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect) OnCorrectSequence();
        else OnWrongSequence();

        currentInput.Clear();
    }

    private void OnCorrectSequence()
    {
        isDone = true;
        isUnlocked = true;

        Debug.Log("<color=green>[Phone Debug]</color> 密码正确！电话已接通。");

        if (QuestManager.Instance != null && !QuestManager.Instance.isPhoneDone)
        {
            QuestManager.Instance.SetPhoneDone();

            // 【关键新增】强行在 ObjectManager 的字典中，将小熊的状态设为 true（可用/未捡起）
            // 这样切回场景1时，ObjectManager 才会允许它显示
            ObjectManager.Instance.ForceSetItemAvailable(ItemName.Bear, true);
        }
    }


    private void OnWrongSequence()
    {
        Debug.Log("<color=red>[Phone Debug]</color> 密码错误！请重新输入。");
    }
}
