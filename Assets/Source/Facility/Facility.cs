using System;
using UnityEngine;

public class Facility : MonoBehaviour
{
    public enum State { PendingConstruction, Building, Built }

    [Header("建造")]
    [SerializeField] private float buildTime = 5f;

    private State currentState = State.PendingConstruction;
    private float buildTimer;
    private SpriteRenderer spriteRenderer;
    private GameObject buildPrompt;

    /// <summary> GAS 属性集，子类通过 InitAttributes 注册属性 </summary>
    protected AttributeSet AttributeSet { get; private set; }

    public State CurrentState => currentState;
    public bool IsBuilt => currentState == State.Built;
    public float BuildProgress => buildTime > 0f ? 1f - buildTimer / buildTime : 1f;
    public event Action<State> OnStateChanged;

    private void Awake()
    {
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        buildPrompt = transform.Find("UI")?.gameObject;
        SetAlpha(0.5f);
        AttributeSet = new AttributeSet();
        InitAttributes();
    }

    /// <summary> 子类重写以注册属性（Health、Attack 等） </summary>
    protected virtual void InitAttributes() { }

    /// <summary> 开始建造倒计时 </summary>
    public void Build()
    {
        buildTimer = buildTime;
        SwitchState(State.Building);
    }

    private void Update()
    {
        if (currentState != State.Building) return;

        buildTimer -= Time.deltaTime;
        if (buildTimer <= 0f)
            CompleteBuilding();
    }

    private void CompleteBuilding()
    {
        SetAlpha(1f);
        buildTimer = 0f;
        SwitchState(State.Built);
        OnBuilt();
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    /// <summary> 建造完成回调，子类重写以启用设施功能 </summary>
    protected virtual void OnBuilt() { }

    /// <summary> 显示建造提示 UI </summary>
    public void ShowPrompt()
    {
        if (currentState != State.PendingConstruction) return;
        if (buildPrompt != null)
            buildPrompt.SetActive(true);
    }

    /// <summary> 隐藏建造提示 UI </summary>
    public void HidePrompt()
    {
        if (buildPrompt != null)
            buildPrompt.SetActive(false);
    }

    private void SwitchState(State newState)
    {
        currentState = newState;

        // 离开 PendingConstruction 时隐藏提示
        if (newState != State.PendingConstruction)
            HidePrompt();

        OnStateChanged?.Invoke(newState);
    }
}

