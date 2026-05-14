using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class TalbertS1 : Interactive
{
    private SpriteRenderer spriteRenderer;
    private DialogueController dialogueController;

    [Header("Sprites")]
    public Sprite holdingDollSprite;   // 初始：抱着娃娃
    public Sprite emptyHandedSprite;   // 中间1：空手 (起始状态)
    public Sprite sideSprite;          // 【新增】中间2：侧身图片
    public Sprite openSprite;          // 最终：完成状态

    [Header("Positions & Animation")]
    public float sidePositionX = 2.0f;     // 侧身图的 X 轴位置 (相对于初始位置偏移)
    public float openPositionX = 4.0f;     // 完成图的 X 轴位置 (相对于初始位置偏移)
    
    public float fadeDuration = 1.0f;      // 每次淡入/淡出的持续时间
    public float minAlpha = 0.2f;          // 【新增】最低透明度 (0 = 完全不可见, 0.2 = 隐约可见)
    public float maxAlpha = 1.0f;          // 【新增】最高透明度

    private Vector3 initialPosition;       // 记录初始位置

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 记录物体在编辑器中的初始位置
        initialPosition = transform.position;

        // 场景初始化时更新一次
        UpdateSpriteBasedOnState();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 每次场景加载后，根据状态刷新
        UpdateSpriteBasedOnState();
    }

    // 【核心】根据全局状态决定显示哪张图、在什么位置
    private void UpdateSpriteBasedOnState()
    {
        // 如果正在播放动画，不要干扰协程
        // 注意：这里简单判断，实际项目中建议用 bool isAnimating 标记
        if (IsInvoking()) return; 

        if (isDone)
        {
            // 最终状态
            spriteRenderer.sprite = openSprite;
            transform.position = new Vector3(initialPosition.x + openPositionX, initialPosition.y, initialPosition.z);
            SetSpriteAlpha(maxAlpha);
        }
        else
        {
            bool hasDollInInventory = InventoryManager.Instance.HasItem(ItemName.Doll);
            
            if (hasDollInInventory)
            {
                // 如果已经拿走了娃娃，但还没给玻璃水，显示空手
                spriteRenderer.sprite = emptyHandedSprite;
                transform.position = initialPosition; 
                SetSpriteAlpha(maxAlpha);
            }
            else
            {
                // 初始状态
                spriteRenderer.sprite = holdingDollSprite;
                transform.position = initialPosition;
                SetSpriteAlpha(maxAlpha);
            }
        }
    }

    private void SetSpriteAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    public override void EmptyClicked()
    {
        if (isDone)
        {
            dialogueController.ShowDialogueFinish();
            return;
        }

        if (!InventoryManager.Instance.HasItem(ItemName.Doll))
        {
            TakeDoll();
        }
        else
        {
            dialogueController.ShowDialogueEmpty();
        }
    }

    public override void CheckItem(ItemName itemName)
    {
        if (isDone)
        {
            dialogueController.ShowDialogueFinish();
            return;
        }

        // 如果还没拿走娃娃，优先拿走娃娃
        if (!InventoryManager.Instance.HasItem(ItemName.Doll))
        {
            TakeDoll();
            return;
        }

        if (itemName == ItemName.Glass)
        {
            // 【关键】启动复杂的淡入淡出协程
            StartCoroutine(PlayGlassSequence());
        }
        else
        {
            dialogueController.ShowDialogueEmpty();
        }
    }

    private void TakeDoll()
    {
        if (emptyHandedSprite != null)
        {
            spriteRenderer.sprite = emptyHandedSprite;
        }
        InventoryManager.Instance.AddItem(ItemName.Doll);
    }

    // 【新增】处理给予 Glass 的完整动画序列
    private IEnumerator PlayGlassSequence()
    {
        // 1. 消耗物品
        EventHandler.CallItemUsedEvent(ItemName.Glass);

        // --- 第一阶段：Empty Handed (左/原位) -> Side Sprite (中/右移) ---
        
        // A. Empty Handed 逐渐变暗 (但不完全消失)
        yield return StartCoroutine(FadeTo(minAlpha));
        
        // B. 切换为侧身图，并移动位置 (此时透明度为 minAlpha，玩家能隐约看到切换瞬间，或者你可以保持 minAlpha 很低)
        spriteRenderer.sprite = sideSprite;
        transform.position = new Vector3(initialPosition.x + sidePositionX, initialPosition.y, initialPosition.z);
        
        // C. 侧身图逐渐变亮
        yield return StartCoroutine(FadeTo(maxAlpha));

        // 等待一小会儿，让玩家看清侧身状态
        yield return new WaitForSeconds(0.5f);

        // --- 第二阶段：Side Sprite (中/右移) -> Open Sprite (更右) ---

        // D. 侧身图逐渐变暗
        yield return StartCoroutine(FadeTo(minAlpha));

        // E. 切换为完成图，并移动到最终位置
        spriteRenderer.sprite = openSprite;
        transform.position = new Vector3(initialPosition.x + openPositionX, initialPosition.y, initialPosition.z);

        // F. 完成图逐渐变亮
        yield return StartCoroutine(FadeTo(maxAlpha));

        // --- 结束 ---
        isDone = true;
        EventHandler.CallItemGivenToNPCEvent(ItemName.Glass);
        dialogueController.ShowDialogueFinish();
    }

    // 【修改】协程：改变透明度到目标值 (通用方法)
    private IEnumerator FadeTo(float targetAlpha)
    {
        float timer = 0;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        spriteRenderer.color = endColor;
    }

    private void ShowHint(string message)
    {
        var toastMgr = FindObjectOfType<ToastManager>();
        if (toastMgr != null) toastMgr.ShowToast(message);
        else Debug.Log($"[Hint] {message}");
    }
}