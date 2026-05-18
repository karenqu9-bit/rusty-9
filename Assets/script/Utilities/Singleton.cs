using UnityEngine;

/// <summary>
/// 通用的单例基类，用于替代之前的 DLL 插件。
/// 确保场景中只有一个实例，并在场景切换时保留 (DontDestroyOnLoad)。
/// 兼容 WebGL 打包。
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 尝试在场景中查找现有实例
                _instance = FindObjectOfType<T>();

                // 如果没找到，创建一个新的 GameObject 并挂载组件
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                    Debug.Log($"[Singleton] Created new instance for {typeof(T).Name}");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 如果已经有一个实例，且不是当前这个，则销毁当前这个（防止重复）
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 设置当前实例
        _instance = this as T;
        
        // 关键：确保场景切换时不销毁该物体
        DontDestroyOnLoad(gameObject);
    }
}