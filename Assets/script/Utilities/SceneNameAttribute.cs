using UnityEngine;

/// <summary>
/// 一个简单的属性标记，用于标识场景名称字段。
/// 注意：此版本不包含自定义 Inspector 下拉菜单功能，仅用于标记和打包兼容性。
/// 如果需要下拉菜单功能，请确保配套的 PropertyDrawer 放在 Editor 文件夹中。
/// </summary>
public class SceneNameAttribute : PropertyAttribute 
{
    // 这里不需要任何代码，它只是一个标记
}