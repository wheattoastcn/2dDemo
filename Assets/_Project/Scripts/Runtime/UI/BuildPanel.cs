using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : UIPanel
{
    [Header("引用")]
    [SerializeField] private GameObject panelRoot;       // 面板视觉根节点
    [SerializeField] private Button toggleButton;        // 手动关闭按钮

    [Header("动态按钮")]
    [SerializeField] private Button buttonPrefab;          // 按钮模板 Prefab
    [SerializeField] private Transform contentParent;       // ScrollView 的 Content

    private BuildManager buildManager; // 从 UIManager 注册中心获取，不再拖拽

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

    //
    private void Start()
    {
        // 面板打开时才绑定 BuildManager（通过 OnOpen）
        if (toggleButton != null)
            toggleButton.onClick.AddListener(() => UIManager.Instance?.HidePanel("BuildPanel"));
    }

    public override void OnOpen(object data = null)
    {
        base.OnOpen(data);

        // 每次打开面板时重新从注册中心获取 BuildManager（关卡重载后是新实例）
        buildManager = UIManager.Instance?.Get<BuildManager>();
        if (buildManager != null)
        {
            buildManager.OnStateChanged -= OnBuildStateChanged;
            buildManager.OnStateChanged += OnBuildStateChanged;
        }

        // 重新生成按钮（设施列表可能变化）
        ClearButtons();
        GenerateButtons();
    }

    public override void OnClose()
    {
        base.OnClose();
        if (buildManager != null)
            buildManager.OnStateChanged -= OnBuildStateChanged;
    }

    //清理资源
    private void OnDestroy()
    {
        if (buildManager != null)
            buildManager.OnStateChanged -= OnBuildStateChanged;
        if (toggleButton != null)
            toggleButton.onClick.RemoveAllListeners();
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

            // 设置按钮文字
            var tmpLabel = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmpLabel != null)
                tmpLabel.text = info.facilityName;
            else
            {
                var label = btn.GetComponentInChildren<Text>(true);
                if (label != null)
                    label.text = info.facilityName;
            }

            // 设置按钮图标：动态创建一个子 Image 撑满按钮
            if (info.buildsprite != null)
            {
                // 查找按钮里是否已有 "Icon" 子物体
                Transform iconTr = btn.transform.Find("Icon");
                Image iconImage;
                if (iconTr != null)
                {
                    iconImage = iconTr.GetComponent<Image>();
                }
                else
                {
                    // 没有则动态创建一个
                    GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                    iconObj.transform.SetParent(btn.transform, false);
                    RectTransform iconRt = (RectTransform)iconObj.transform;
                    iconRt.anchorMin = Vector2.zero;
                    iconRt.anchorMax = Vector2.one;
                    iconRt.offsetMin = new Vector2(5, 25);   // 留出底部给文字
                    iconRt.offsetMax = new Vector2(-5, -5);
                    iconImage = iconObj.GetComponent<Image>();
                }

                if (iconImage != null)
                {
                    iconImage.sprite = info.buildsprite;
                    iconImage.preserveAspect = true;
                    iconImage.raycastTarget = false;  // 不拦截点击
                }
            }

            string name = info.facilityName;
            btn.onClick.AddListener(() => buildManager.SelectFacility(name));
        }
    }

    /// <summary> 清空之前生成的按钮 </summary>
    private void ClearButtons()
    {
        if (contentParent == null) return;
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
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
}
