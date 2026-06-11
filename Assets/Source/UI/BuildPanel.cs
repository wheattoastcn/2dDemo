using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button toggleButton;

    [Header("动态按钮")]
    [SerializeField] private Button buttonPrefab;          // 按钮模板 Prefab
    [SerializeField] private Transform contentParent;       // ScrollView 的 Content

    private bool isOpen;

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
    }
}
