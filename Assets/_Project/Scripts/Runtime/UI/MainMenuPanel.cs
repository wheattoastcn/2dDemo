using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : UIPanel
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        Debug.Log($"[MainMenuPanel] Start 执行，当前 activeSelf = {gameObject.activeSelf}");

        // 绑定按钮事件
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnStartClicked()
    {
        // 通知 GameManager 加载关卡（这里假设第一关叫 "Level1"）
        GameManager.Instance?.LoadLevel("Launcher");
    }

    private void OnSettingsClicked()
    {
        // 打开设置面板（假设已注册好 SettingsPanel）
        UIManager.Instance?.ShowPanel("SettingsPanel");
    }

    private void OnQuitClicked()
    {
        // 退出游戏（在编辑器里不会真正退出）
        Application.Quit();
    }

    private void OnDestroy()
    {
        // 清理监听，避免内存泄漏
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    public override void Show()
    {
        base.Show();
        Debug.Log($"[MainMenuPanel] Show 执行，gameObject.activeSelf = {gameObject.activeSelf}");
        Debug.Log($"[MainMenuPanel] CanvasGroup alpha = {canvasGroup.alpha}, interactable = {canvasGroup.interactable}");
    }
}