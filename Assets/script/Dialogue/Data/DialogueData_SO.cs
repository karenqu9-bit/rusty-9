using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData_SO", menuName = "Dialogue/DialogueData_SO")]
public class DialogueData_SO : ScriptableObject
{
    public List<string> dialogueList;
        // 【新增】为该段对话专属的音效
    public AudioClip dialogueSound;
}