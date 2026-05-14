using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;

    public void UpdateItemName(ItemName itemName)
    {
        itemNameText.text = itemName switch
        {
            ItemName.Bear =>"玩具小熊",
            ItemName.Glass =>"隐形眼镜液",
            ItemName.Mask =>"面具",
            ItemName.Doll =>"鬼娃娃",
            _=>""
        };
    }
}
