using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteraction : Interactive
{
    [Header("Sprites")]
    public Sprite emptyBedSprite;   // 初始状态：空床
    public Sprite dollInBedSprite;  // 目标状态：娃娃在床上
    
    // 【新增】用于模拟晃动的中间帧图片
    public Sprite rockingSprite1;   // 晃动帧1
    public Sprite rockingSprite2;   // 晃动帧2

    private SpriteRenderer spriteRenderer;
    
    // 【新增】像 CliffS2 一样，本地记录最后选中的物品
    private ItemName lastSelectedItem = ItemName.None;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (emptyBedSprite != null)
        {
            spriteRenderer.sprite = emptyBedSprite;
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        // 【新增】监听物品选中事件
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        // 【新增】取消监听
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    // 【新增】更新本地记录的物品
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (isSelected && itemDetails != null)
        {
            lastSelectedItem = itemDetails.itemName;
        }
        else if (!isSelected)
        {
            lastSelectedItem = ItemName.None;
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (isDone && dollInBedSprite != null)
        {
            spriteRenderer.sprite = dollInBedSprite;
        }
        else if (!isDone && emptyBedSprite != null)
        {
            spriteRenderer.sprite = emptyBedSprite;
        }
    }

    public override void EmptyClicked()
    {
        if (isDone)
        {
            // 【修改】如果床上已经有娃娃，播放晃动动画
            PlayRockingAnimation();
        }
        else
        {
            Debug.Log("[Bed] The bed is empty. Maybe I should put something here?");
        }
    }

    public override void CheckItem(ItemName itemName)
    {
        // 【修改】不再依赖传入的 itemName，而是使用本地记录的 lastSelectedItem
        // 这样即使 CursorManager 传参有误，只要事件触发正常，逻辑就能跑通
        
        if (isDone) 
        {
            // 如果已经放好了，再次手持物品点击也播放晃动效果（可选）
            PlayRockingAnimation();
            return;
        }

        // 【核心逻辑】检查手持的是否是 Doll
        if (lastSelectedItem == ItemName.Doll)
        {
            PlaceDollOnBed();
        }
        else
        {
            // 可选：提示玩家手持了错误的物品
            if (lastSelectedItem != ItemName.None)
            {
                Debug.Log($"[Bed] I can't put {lastSelectedItem} on the bed.");
            }
        }
    }

    private void PlaceDollOnBed()
    {
        if (dollInBedSprite != null)
        {
            spriteRenderer.sprite = dollInBedSprite;
        }

        isDone = true;

        // 通知背包移除物品
        // 注意：这里传入 lastSelectedItem 确保移除的是正确的物品
        EventHandler.CallItemUsedEvent(lastSelectedItem);

        Debug.Log("[Bed] Doll placed on the bed!");
    }

    // 【新增】播放摇床晃动动画协程
    private void PlayRockingAnimation()
    {
        // 如果没有设置晃动图片，或者当前没有娃娃，则不播放
        if (rockingSprite1 == null || rockingSprite2 == null || !isDone) 
            return;

        // 如果正在播放，先停止之前的，避免冲突
        StopCoroutine("RockingRoutine");
        StartCoroutine("RockingRoutine");
    }

    private IEnumerator RockingRoutine()
    {
        // 保存当前静态图，以便动画结束后恢复
        Sprite originalSprite = spriteRenderer.sprite;

        // 循环次数：比如晃动 3 个周期
        int cycles = 2;
        float frameDuration = 0.5f; // 每帧切换的时间间隔

        for (int i = 0; i < cycles; i++)
        {
            // 序列：原图 -> 晃动1 -> 原图 -> 晃动2 -> 原图
            yield return new WaitForSeconds(frameDuration);
            spriteRenderer.sprite = rockingSprite1;
            
            yield return new WaitForSeconds(frameDuration);
            spriteRenderer.sprite = originalSprite; // 回到中间状态
            
            yield return new WaitForSeconds(frameDuration);
            spriteRenderer.sprite = rockingSprite2;
            
            yield return new WaitForSeconds(frameDuration);
            spriteRenderer.sprite = originalSprite; // 回到中间状态
        }

        // 确保最后定格在 dollInBedSprite
        if (dollInBedSprite != null)
        {
            spriteRenderer.sprite = dollInBedSprite;
        }
    }
}