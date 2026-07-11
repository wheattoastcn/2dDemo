using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : UIPanel
{
    [Header("引用")]
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private GameObject panelRoot;       // 面板视觉根节点
    [SerializeField] private Button toggleButton;        // 手动关闭按钮

    [Header("动态按钮")]
    [SerializeField] private Button buttonPrefab;          // 按钮模板 Prefab
    [SerializeField] private Transform contentParent;       // ScrollView 的 Content

    /*private bool isOpen;

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(TogglePanel);

        GenerateButtons();

        if (buildManager != null)
            buildManager.OnStateChanged += OnBuildStateChanged;

        panelRoot.SetActive(false);
    }

    private void OnDestroy()
    {
        if (buildManager != null)
            buildManager.OnStateChanged -= OnBuildStateChanged;
    }

    /// <summary> 从 facilityData 动态生成按钮 </summary>
    private void GenerateButtons()
    {
        foreach (var info in buildManager.FacilityList)
        {
            Button btn = Instantiate(buttonPrefab, contentParent);
            // 设按钮文字（兼容 TMP 和旧版 Text）
            var tmpLabel = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (tmpLabel != null)
                tmpLabel.text = info.facilityName;
            else
            {
                var label = btn.GetComponentInChildren<Text>(true);
                if (label != null)
                    label.text = info.facilityName;
            }

            // 闭包变量固定，避免 foreach 经典 bug
            string name = info.facilityName;
            btn.onClick.AddListener(() => buildManager.SelectFacility(name));
        }
    }

    private void TogglePanel()
    {
        isOpen = !isOpen;
        panelRoot.SetActive(isOpen);
    }

    private void OnBuildStateChanged(BuildManager.State state)
    {
        if (state == BuildManager.State.Previewing)
        {
            isOpen = false;
            panelRoot.SetActive(false);
        }
    }*/


    private void Start()
    {
        // 原有动态生成
        GenerateButtons();

        // 可选：如果 panel 内有个关闭按钮，就绑定
        if (toggleButton != null)
            toggleButton.onClick.AddListener(() => UIManager.Instance?.HidePanel("BuildPanel"));

        // 监听 BuildManager 的状态变化
        if (buildManager != null)
            buildManager.OnStateChanged += OnBuildStateChanged;

        // 初始隐藏交给 UIManager.Awake 处理，这里无需手动 SetActive
    }

    public override void Show()
    {
        base.Show();                     // 激活整个面板物体 + CanvasGroup 交互
        if (panelRoot != null)
            panelRoot.SetActive(true);   // 如果你的面板结构有额外的根节点，可同时激活
    }

    public override void Hide()
    {
        base.Hide();
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void GenerateButtons()
    {
        if (buildManager == null) return;

        foreach (var info in buildManager.FacilityList)
        {
            Button btn = Instantiate(buttonPrefab, contentParent);
            var tmpLabel = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpLabel != null)
                tmpLabel.text = info.facilityName;
            else
            {
                var label = btn.GetComponentInChildren<Text>(true);
                if (label != null)
                    label.text = info.facilityName;
            }

            string name = info.facilityName;
            btn.onClick.AddListener(() => buildManager.SelectFacility(name));
        }
    }

    private void OnBuildStateChanged(BuildManager.State state)
    {
        // 当进入预览状态时，自动关闭建造面板
        if (state == BuildManager.State.Previewing)
        {
            UIManager.Instance?.HidePanel("BuildPanel");
        }
    }

    private void OnDestroy()
    {
        if (buildManager != null)
            buildManager.OnStateChanged -= OnBuildStateChanged;
        if (toggleButton != null)
            toggleButton.onClick.RemoveAllListeners();
    }
}
}
