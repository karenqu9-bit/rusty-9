using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object9Interaction : Interactive
{
    public override void EmptyClicked()
    {
        // 点击物体9时，触发解锁事件，传入兔子foot的物体名称
        EventHandler.CallGenericUnlockEvent("changpian");
    }
}
