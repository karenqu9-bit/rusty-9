using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talbert : Interactive
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    public Sprite openSprite;

    private void Awake()
    {
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
        if(isDone)
        {
            spriteRenderer.sprite = openSprite;
            coll.enabled = false;
        }
    }


    protected override void OnclickedAction()
    {
        spriteRenderer.sprite = openSprite;
    }
}
