using UnityEngine;

public class PuzzleCompletionHandler : MonoBehaviour
{
    private void OnEnable()
    {
        PuzzleCompletedEvent.OnPuzzleCompletedEvent += OnPuzzleCompleted;
    }

    private void OnDisable()
    {
        PuzzleCompletedEvent.OnPuzzleCompletedEvent -= OnPuzzleCompleted;
    }


    private void OnPuzzleCompleted()
    {
        Debug.Log("[PuzzleCompletionHandler] 解密完成，执行后续逻辑...");
        // 这里可以添加解密完成后的逻辑，比如打开门、播放动画等
    }
}
