using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Transform levelRoot;   // 拖入场景中的 LevelRoot 节点
    [SerializeField] private string startLevel;     // 测试直接填关卡名，如 "Level1"

    private string currentLevelName;

    private void Awake()
    {
        // 单例：保证全局唯一
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 如果有配置测试关卡，直接加载；否则显示主菜单
        Debug.Log("[GameManager] Start 执行");
        if (!string.IsNullOrEmpty(startLevel))
        {
            Debug.Log($"[GameManager] startLevel 不为空，直接加载关卡：{startLevel}");
            LoadLevel(startLevel);
        }
        else
        {
            Debug.Log("[GameManager] startLevel 为空，调用 ShowMainMenu()");
            if (UIManager.Instance != null)
                UIManager.Instance.ShowMainMenu();
            else
                Debug.LogError("[GameManager] UIManager.Instance 为 null！");
        }
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadLevelAsync(levelName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        // 可选：播放过场动画
        UIManager.Instance?.ShowTransition(true);

        // 卸载之前的关卡
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            Scene oldScene = SceneManager.GetSceneByName(currentLevelName);
            if (oldScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(oldScene);
            }
        }

        // 异步加载新关卡为 Additive
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // 将关卡场景的根物体移动到 levelRoot 下
        Scene newScene = SceneManager.GetSceneByName(levelName);
        if (newScene.isLoaded && levelRoot != null)
        {
            GameObject[] rootObjects = newScene.GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                obj.transform.SetParent(levelRoot, false);
            }
        }

        currentLevelName = levelName;

        // 通知 UIManager 关卡已加载
        UIManager.Instance?.OnLevelLoaded(levelName);

        // 关闭过场
        UIManager.Instance?.ShowTransition(false);

        // 在 LoadLevelAsync 中，成功加载后记录
        lastLevelName = levelName;
        currentLevelName = levelName;
        IsGameOver = false;
    }


    //PausePanel调用

    // 是否已加载关卡（用于判断是否可以暂停等）
    public bool IsLevelLoaded => !string.IsNullOrEmpty(currentLevelName);

    public void ReturnToMainMenu()
    {
        // 1. 卸载关卡  // 停止可能正在运行的加载协程（简单处理：直接卸载）
        if (!string.IsNullOrEmpty(currentLevelName))
        {
            Scene oldScene = SceneManager.GetSceneByName(currentLevelName);
            if (oldScene.isLoaded)
                SceneManager.UnloadSceneAsync(oldScene);
            currentLevelName = null;
        }

        // 2. 清空关卡对象    // 清理关卡相关对象（如果 levelRoot 下有物体，全部销毁）
        if (levelRoot != null)
        {
            foreach (Transform child in levelRoot)
                Destroy(child.gameObject);
        }

        IsGameOver = false;

        // 3. 隐藏所有游戏内 UI
        UIManager.Instance?.HideAllGameplayPanels();

        // 4. 显示主菜单
        UIManager.Instance?.ShowMainMenu();
    }



    //GameOverPanel调用

    private string lastLevelName; // 记录上次加载的关卡名，用于重新开始

    public bool IsGameOver { get; private set; }

    public void GameOver(bool isVictory)
    {
        if (IsGameOver) return;
        IsGameOver = true;

        // 隐藏其他游戏内面板（保留 GameOverPanel 独占）
        UIManager.Instance?.HidePanel("HUDPanel");
        UIManager.Instance?.HidePanel("PausePanel");
        UIManager.Instance?.HidePanel("BuildPanel");

        // 显示结果面板
        UIManager.Instance?.ShowPanel("GameOverPanel", isVictory);
    }

    public void RestartLevel()
    {
        if (string.IsNullOrEmpty(lastLevelName))
        {
            Debug.LogError("没有可重新开始的关卡！");
            return;
        }
        IsGameOver = false;
        LoadLevel(lastLevelName);
    }
}