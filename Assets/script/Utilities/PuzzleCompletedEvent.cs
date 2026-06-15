using UnityEngine;
using UnityEngine.Events;

public static class PuzzleCompletedEvent
{
    public static event UnityAction OnPuzzleCompletedEvent;

    public static void CallPuzzleCompletedEvent()
    {
        OnPuzzleCompletedEvent?.Invoke();
    }
}
