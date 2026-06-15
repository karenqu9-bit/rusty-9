using UnityEngine;

public class PhoneKeyClick : MonoBehaviour
{
    public PhoneInteraction phoneInteraction; // 拖入电话底图物体
    public int keyNumber; // 这个按键代表的数字

    private void OnMouseDown()
    {
        if (phoneInteraction != null)
        {
            phoneInteraction.OnKeyPressed(keyNumber);
        }
        else
        {
            Debug.LogError("PhoneKeyClick: 未分配 PhoneInteraction！");
        }
    }
}
