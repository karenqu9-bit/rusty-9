using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class TalbertS1 : Interactive
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    public Sprite openSprite;
    private DialogueController dialogueController;

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
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
        if (isDone)
        {
            spriteRenderer.sprite = openSprite;
            // coll.enabled = false; //
        }
    }

    public override void EmptyClicked()
    {
        if (isDone)
            dialogueController.ShowDialogueFinish();
        else
            //对话内容A
            dialogueController.ShowDialogueEmpty();
    }
    protected override void OnclickedAction()
    {

        dialogueController.ShowDialogueFinish();
        spriteRenderer.sprite = openSprite;
        EventHandler.CallItemGivenToNPCEvent(ItemName.Glass);
    }
}
