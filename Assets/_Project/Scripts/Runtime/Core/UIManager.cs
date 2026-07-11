using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // 所有面板的根节点（在Canvas下创建空节点 Panels）
    [SerializeField] private Transform panelsRoot;

    // 编辑器拖入的面板列表，或者通过代码动态加载（初期先拖入）
    [SerializeField] private List<UIPanel> allPanels = new List<UIPanel>();

    private Dictionary<string, UIPanel> panelDict = new Dictionary<string, UIPanel>();
    private Stack<UIPanel> panelStack = new Stack<UIPanel>();

    // 方便直接引用常用面板
    [Header("常用面板快捷方式")]
    [SerializeField] private UIPanel mainMenuPanel;
    [SerializeField] private UIPanel hudPanel;
    [SerializeField] private UIPanel buildPanel;
    [SerializeField] private UIPanel pausePanel;
    [SerializeField] private UIPanel gameOverPanel;

    private void Awake()
    {
        Instance = this;

        // 初始化面板字典
        foreach (var panel in allPanels)
        {
            if (panel != null)
            {
                string key = panel.GetType().Name;  // 用类名作为键，也可以自定义
                if (!panelDict.ContainsKey(key))
                    panelDict.Add(key, panel);
                // panel.Hide();  // 初始全部隐藏// 不再需要，面板已在场景中设为禁用
            }
        }

        // 也可以把快捷引用注册进去
        RegisterPanel("MainMenuPanel", mainMenuPanel);
        RegisterPanel("HUDPanel", hudPanel);
        RegisterPanel("BuildPanel", buildPanel);
        RegisterPanel("PausePanel", pausePanel);
        RegisterPanel("GameOverPanel", gameOverPanel);

        Debug.Log($"[UIManager] Awake 完成，面板字典数量：{panelDict.Count}");
        foreach (var kvp in panelDict)
            Debug.Log($"[UIManager] 面板键：{kvp.Key}, 实例名称：{kvp.Value.name}");
    }

    private void RegisterPanel(string name, UIPanel panel)
    {
        if (panel != null && !panelDict.ContainsKey(name))
        {
            panelDict.Add(name, panel);
            panel.Hide();
        }
    }

    // 打开一个面板（会关闭当前栈顶面板？提供两种模式：替换或叠加）
    public void ShowPanel(string panelName, object data = null, bool addToStack = true)
    {
        Debug.Log($"[UIManager] ShowPanel 被调用，面板名：{panelName}");
        if (!panelDict.TryGetValue(panelName, out UIPanel panel))
        {
            Debug.LogWarning($"[UIManager] 字典中找不到面板：{panelName}");
            //Debug.LogWarning($"Panel {panelName} not found!");
            foreach (var key in panelDict.Keys)
                Debug.Log("当前字典键: " + key);
            return;
        }

        Debug.Log($"[UIManager] 找到面板 {panelName}，准备调用 Show()");

        // 如果栈里有面板，可以选择隐藏上一个，也可以保留（用于弹窗覆盖）
        if (addToStack && panelStack.Count > 0)
        {
            UIPanel currentTop = panelStack.Peek();
            // 如果当前面板不是自己要打开的面板，则隐藏它（一般情况）
            if (currentTop != panel)
                currentTop.Hide();
        }

        panel.Show();
        panel.OnOpen(data);

        if (addToStack)
        {
            // 如果已在栈中，先移除再推入，保持栈顺序正确
            if (panelStack.Contains(panel))
                RemovePanelFromStack(panel);
            panelStack.Push(panel);
        }
    }

    // 关闭最近打开的面板，返回上一层
    public void CloseCurrentPanel()
    {
        if (panelStack.Count == 0) return;
        UIPanel top = panelStack.Pop();
        top.Hide();
        top.OnClose();

        // 显示新栈顶
        if (panelStack.Count > 0)
        {
            UIPanel newTop = panelStack.Peek();
            newTop.Show();
        }
    }

    // 直接关闭某个面板
    public void HidePanel(string panelName)
    {
        if (panelDict.TryGetValue(panelName, out UIPanel panel))
        {
            RemovePanelFromStack(panel);    // 如果用到栈，移除
            panel.Hide();                   // 调用基类 Hide，里面会 SetActive(false)
            panel.OnClose();
        }
    }

    private void RemovePanelFromStack(UIPanel panel)
    {
        if (panelStack.Contains(panel))
        {
            Stack<UIPanel> temp = new Stack<UIPanel>();
            while (panelStack.Count > 0)
            {
                UIPanel p = panelStack.Pop();
                if (p != panel) temp.Push(p);
            }
            while (temp.Count > 0)
                panelStack.Push(temp.Pop());
        }
    }

    // 用于 GameManager 调用的主菜单显示
    public void ShowMainMenu()
    {
        Debug.Log("[UIManager] ShowMainMenu 被调用，准备调用ShowPanel()");
        ShowPanel("MainMenuPanel", null, true);
    }

    // 关卡加载完成时显示 HUD
    public void OnLevelLoaded(string levelName)
    {
        HidePanel("MainMenuPanel");             // 隐藏主菜单
        ShowPanel("HUDPanel", null, false);     // 不推入栈，HUD常驻底层
    }

    public void ShowTransition(bool show)
    {
        // 可以控制一个全屏遮罩面板
    }



    [Header("暂停面板")]
    [SerializeField] private string pausePanelName = "PausePanel";

    private void Update()
    {
        // 如果游戏未开始，或主菜单显示中，不响应 Esc
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return; // 游戏结束时不响应 Esc

        if (GameManager.Instance != null && !GameManager.Instance.IsLevelLoaded)
            return; // 未加载关卡也不响应

        // 如果暂停面板已经打开，不重复打开（按 Esc 会继续游戏）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelStack.Count > 0 && panelStack.Peek().GetType().Name == pausePanelName)
            {
                // 暂停面板已是栈顶，按 Esc 关闭
                CloseCurrentPanel();
            }
            else
            {
                ShowPanel(pausePanelName);
            }
        }
    }


    /// <summary>
    /// 隐藏所有与游戏进程相关的面板（HUD、暂停、建造等），但不碰主菜单和设置等全局面板
    /// </summary>
    public void HideAllGameplayPanels()
    {
        // 直接隐藏已知的游戏面板，也可以遍历栈清理
        HidePanel("HUDPanel");
        HidePanel("PausePanel");
        HidePanel("BuildPanel");   // 如果有了建造面板
        HidePanel("GameOverPanel");
        // 清空面板栈（因为这些面板都不该再存在）
        panelStack.Clear();
    }

}