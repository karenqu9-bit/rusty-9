using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [Header("UI Elements")]
    public GameObject startText; // 拖入开始文本的引用

    [SceneName] public string startScene;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;

    // 【新增】在 Inspector 中拖入你的脚步声音效文件
    [Header("Audio Settings")]
    public AudioClip footstepClip;

    private bool isFade;
    private bool canTransition;
    private bool isFirstLoad = true; // 新增：标记是否是第一次加载场景

    // 【新增】私有 AudioSource 引用
    private AudioSource audioSource;

    // 【修改】不再重写 Awake，而是使用 Start 或 OnEnable 进行额外初始化
    // 因为 Singleton 基类已经在 Awake 里处理了 Instance 和 DontDestroyOnLoad
    private void Start()
    {
        // 【关键】确保当前物体上有 AudioSource，如果没有则自动添加
        if (!TryGetComponent(out audioSource))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置 AudioSource 属性
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;

        // 显示开始文本
        if (startText != null)
        {
            startText.SetActive(true);
        }

        // 开始初始场景过渡 - 改为加载开始场景
        StartCoroutine(TransitionToScene(string.Empty, "StartScene"));
    }
    private void Update()
    {
        // 检测用户是否点击或触摸屏幕
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            // 获取当前场景名称
            string currentScene = SceneManager.GetActiveScene().name;

            // 如果是在开始场景，则开始游戏
            if (currentScene == "StartScene")
            {
                // 隐藏开始文本
                if (startText != null)
                {
                    startText.SetActive(false);
                }

                // 开始游戏场景过渡
                StartCoroutine(TransitionToScene("StartScene", startScene));
            }
        }
    }


    private void OnEnable()
    {
        EventHandler.GameStateChangeEvent += OnGameStateChangeEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameStateChangeEvent -= OnGameStateChangeEvent;
    }

    private void OnGameStateChangeEvent(GameState gameState)
    {
        canTransition = gameState == GameState.GamePlay;
    }

    public void Transition(string from, string to)
    {
        if (!isFade && canTransition)
            StartCoroutine(TransitionToScene(from, to));
    }

    private IEnumerator TransitionToScene(string from, string to)
    {
        // 【新增】在开始淡出（变黑）时播放脚步声
        // 只有不是第一次加载时才播放脚步声
        if (!isFirstLoad)
        {
            PlayFootsteps();
        }
        else
        {
            isFirstLoad = false; // 标记第一次加载已完成
        }

        yield return Fade(1);

        if (from != string.Empty)
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return SceneManager.UnloadSceneAsync(from);
        }

        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
        EventHandler.CallAfterSceneLoadedEvent();

        yield return Fade(0);
    }

    // 【新增】播放脚步声的方法
    private void PlayFootsteps()
    {
        // 添加额外的空值检查
        if (footstepClip != null && audioSource != null && audioSource.isActiveAndEnabled)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetAlpha">1是黑，0是透明</param>
    /// <returns></returns>
    private IEnumerator Fade(float targetAlpha)
    {
        isFade = true;
        fadeCanvasGroup.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;

        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false;
        isFade = false;
    }
}