using UnityEngine;
using UnityEngine.UI;

public class PausePanel : UIPanel
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        resumeButton?.onClick.AddListener(OnResumeClicked);
        mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
        quitButton?.onClick.AddListener(OnQuitClicked);
    }

    public override void Show()
    {
        base.Show();
        Time.timeScale = 0f;    // 暂停游戏
    }

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;    // 恢复游戏
    }

    private void OnResumeClicked()
    {
        // 关闭当前面板（弹出栈），会自动调用 Hide() 恢复时间
        UIManager.Instance?.CloseCurrentPanel();
    }

    private void OnMainMenuClicked()
    {
        // 恢复时间，然后返回主菜单
        Time.timeScale = 1f;
        GameManager.Instance?.ReturnToMainMenu();
    }

    private void OnQuitClicked()
    {
        // 退出游戏（编辑器内无效，但打包后正常）
        MainMenuPanel.QuitGame();  // 复用同一退出方法
    }

    private void OnDestroy()
    {
        resumeButton?.onClick.RemoveAllListeners();
        mainMenuButton?.onClick.RemoveAllListeners();
        quitButton?.onClick.RemoveAllListeners();
        // 防止意外退出时时间被冻结，强制恢复
        Time.timeScale = 1f;
    }
}