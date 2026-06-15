using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiushengjiInteraction : Interactive
{
    [Header("Sprites")]
    public Sprite defaultSprite;       // 初始状态：空留声机
    public Sprite withZhizhenSprite;   // 中间状态1：留声机 w zhizhen
    public Sprite withChangpianSprite; // 中间状态2：留声机 w changpian
    public Sprite completeSprite;      // 最终状态：留声机2

    private SpriteRenderer spriteRenderer;
    private ItemName lastSelectedItem = ItemName.None;

    // 记录是否已经放置了指针和唱片
    private bool hasZhizhen = false;
    private bool hasChangpian = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        UpdateSprite();
    }

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

    public override void EmptyClicked()
    {
        Debug.Log("[Liushengji] 空手点击了留声机");
    }

    public override void CheckItem(ItemName itemName)
    {
        // 如果已经完成，不再响应物品交互
        if (isDone) return;

        if (lastSelectedItem == ItemName.zhizhen && !hasZhizhen)
        {
            hasZhizhen = true;
            EventHandler.CallItemUsedEvent(ItemName.zhizhen);
            UpdateSprite();
        }
        else if (lastSelectedItem == ItemName.changpian && !hasChangpian)
        {
            hasChangpian = true;
            EventHandler.CallItemUsedEvent(ItemName.changpian);
            UpdateSprite();
        }
        else
        {
            Debug.Log("[Liushengji] 无法使用该物品");
        }
    }

    private void UpdateSprite()
    {
        // 如果两者都放置了，变成最终状态
        if (hasZhizhen && hasChangpian)
        {
            if (completeSprite != null)
            {
                spriteRenderer.sprite = completeSprite;
            }
            isDone = true;
        }
        // 只放置了指针
        else if (hasZhizhen)
        {
            if (withZhizhenSprite != null)
            {
                spriteRenderer.sprite = withZhizhenSprite;
            }
        }
        // 只放置了唱片
        else if (hasChangpian)
        {
            if (withChangpianSprite != null)
            {
                spriteRenderer.sprite = withChangpianSprite;
            }
        }
        // 都没放置，显示默认图片
        else if (defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    // 供 ObjectManager 查询指针状态
    public bool GetZhizhenState()
    {
        return hasZhizhen;
    }

    // 供 ObjectManager 设置指针状态
    public void SetZhizhenState(bool state)
    {
        hasZhizhen = state;
    }

    // 供 ObjectManager 查询唱片状态
    public bool GetChangpianState()
    {
        return hasChangpian;
    }

    // 供 ObjectManager 设置唱片状态
    public void SetChangpianState(bool state)
    {
        hasChangpian = state;
    }
}
