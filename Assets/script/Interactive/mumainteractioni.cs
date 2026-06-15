using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MumaInteraction : Interactive
{
    [Header("Rocking Animation")]
    public float rockAngle = 15f;          // 晃动的最大倾斜角度
    public float rockDuration = 0.2f;      // 每次倾斜花费的时间
    // 记录是否已经完成过摇晃
    private bool hasRocked = false;

    private Quaternion initialRotation;    // 记录初始旋转角度
    private void Awake()
    {
        // 记录木马的初始旋转角度
        initialRotation = transform.rotation;
    }
    public override void EmptyClicked()
    {
        // 无论是否完成，点击均播放摇晃动画
        PlayMumaRockingAnimation();

        // 如果需要仅摇晃一次，可取消下面这行的注释
        // isDone = true; 
    }

    // 播放木马摇晃动画
    private void PlayMumaRockingAnimation()
    {
        StopCoroutine("MumaRockingRoutine");
        StartCoroutine("MumaRockingRoutine");
    }

    private IEnumerator MumaRockingRoutine()
    {
        // 获取物体下边线中心点在本地坐标系中的位置
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        Vector3 pivotLocalPosition = transform.InverseTransformPoint(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z));

        float startTime = Time.time;
        float totalDuration = rockDuration * 4f;

        while (Time.time - startTime < totalDuration)
        {
            float progress = (Time.time - startTime) / totalDuration;
            float currentAngle = Mathf.Sin(progress * 2f * 2f * Mathf.PI) * rockAngle;

            // 计算下边线中心点在世界空间中的初始坐标
            Vector3 pivotWorldPosition = transform.TransformPoint(pivotLocalPosition);

            // 应用Z轴旋转
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z + currentAngle);

            // 计算旋转后下边线中心点的新世界坐标
            Vector3 newPivotWorldPosition = transform.TransformPoint(pivotLocalPosition);

            // 偏移物体位置，使得下边线中心点与旋转前保持一致
            transform.position += pivotWorldPosition - newPivotWorldPosition;

            yield return null;
        }

        // 确保最终停留在初始位置
        transform.rotation = initialRotation;
        // 如果是第一次完成摇晃，触发通用解锁事件让changpian出现
        if (!hasRocked)
        {
            hasRocked = true;
            EventHandler.CallGenericUnlockEvent("tuzifoot");
        }
    }

}

