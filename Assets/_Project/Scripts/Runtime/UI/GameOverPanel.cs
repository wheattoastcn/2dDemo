using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : UIPanel
{
    [Header("胜利/失败容器")]
    [SerializeField] private GameObject victoryContent;
    [SerializeField] private GameObject defeatContent;

    [Header("按钮")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private bool isVictory;

    public override void OnOpen(object data = null)
    {
        // data 应为 bool 类型：true 表示胜利，false 表示失败
        if (data is bool victory)
        {
            isVictory = victory;
        }
        else
        {
            // 默认失败
            isVictory = false;
        }

        // 根据结果切换显示
        if (victoryContent != null)
            victoryContent.SetActive(isVictory);
        if (defeatContent != null)
            defeatContent.SetActive(!isVictory);
    }

    private void Start()
    {
        restartButton?.onClick.AddListener(OnRestartClicked);
        mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f; // 游戏暂停
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f; // 恢复时间流速（若此时不应恢复，由其他逻辑控制）
    }

    private void OnRestartClicked()
    {
        Time.timeScale = 1f;
        // 重新加载当前关卡（GameManager 中需记录当前关卡名）
        GameManager.Instance?.RestartLevel();
        // 隐藏自身
        UIManager.Instance?.HidePanel("GameOverPanel");
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.ReturnToMainMenu();
    }

    private void OnDestroy()
    {
        restartButton?.onClick.RemoveAllListeners();
        mainMenuButton?.onClick.RemoveAllListeners();
        Time.timeScale = 1f; // 安全恢复
    }
}