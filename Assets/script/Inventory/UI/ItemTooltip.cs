using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    // 【新增】单例实例，方便全局访问
    public static ItemTooltip Instance { get; private set; }

    public TMP_Text itemNameText;

    private void Awake()
    {
        // 【新增】单例初始化逻辑
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 防止场景中有多个 Tooltip
        }
        else
        {
            Instance = this;
        }
        
        // 确保初始状态是隐藏的
        gameObject.SetActive(false);
    }

    public void UpdateItemName(ItemName itemName)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemName switch
            {
                ItemName.Bear => "玩具小熊",
                ItemName.Glass => "隐形眼镜液",
                ItemName.Mask => "面具",
                ItemName.Doll => "鬼娃娃",
                ItemName.EmptyCase => "空眼镜盒",
                ItemName.FullCase => "满眼镜盒",
                ItemName.zhizhen => "留声机指针",
                ItemName.changpian => "留声机唱片",
                ItemName.tuzifoot => "兔子脚",
                ItemName.E => "E",
                ItemName.X => "X",
                ItemName.I => "I",
                ItemName.T => "T",



                _ => ""
            };
        }
    }
}