using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : UIPanel
{
    [Header("金币")]
    [SerializeField] private Text goldText;

    [Header("波次")]
    [SerializeField] private Text waveText;

    [Header("血量")]
    [SerializeField] private Slider healthSlider;

    [Header("按钮")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button buildButton;

    private void Start()
    {
        pauseButton?.onClick.AddListener(OnPauseClicked);
        buildButton?.onClick.AddListener(OnBuildClicked);
    }

    public void UpdateGold(int amount)
    {
        if (goldText) goldText.text = amount.ToString();
    }

    public void UpdateWave(int current, int total)
    {
        if (waveText) waveText.text = $"波次 {current}/{total}";
    }

    public void UpdateHealth(float percent)
    {
        if (healthSlider) healthSlider.value = percent;
    }

    private void OnPauseClicked()
    {
        UIManager.Instance?.ShowPanel("PausePanel");
    }

    private void OnBuildClicked()
    {
        UIManager.Instance?.ShowPanel("BuildPanel");
    }

    private void OnDestroy()
    {
        pauseButton?.onClick.RemoveAllListeners();
        buildButton?.onClick.RemoveAllListeners();
    }
}