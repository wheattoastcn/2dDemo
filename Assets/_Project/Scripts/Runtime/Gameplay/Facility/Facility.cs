using System;
using UnityEngine;

public class Facility : MonoBehaviour
{
    public enum State { PendingConstruction, Building, Built }

    [Header("设施数据")]
    [SerializeField] private FacilityInfo facilityInfo;

    [Header("视觉")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private State currentState = State.PendingConstruction;
    private float buildTimer;

    /// <summary> 当前状态 </summary>
    public State CurrentState => currentState;

    /// <summary> 是否已建造完成 </summary>
    public bool IsBuilt => currentState == State.Built;

    /// <summary> 建造进度 (0~1) </summary>
    public float BuildProgress => facilityInfo.buildTime > 0f
        ? 1f - buildTimer / facilityInfo.buildTime
        : 1f;

    /// <summary> 状态变化事件 </summary>
    public event Action<State> OnStateChanged;

    private void Awake()
    {
        
    }

    /// <summary> 注入设施数据并开始建造 </summary>
    public void InitAndBuild(FacilityInfo info)
    {
        facilityInfo = info;
        Build();
    }

    /// <summary> 开始建造倒计时 </summary>
    public void Build()
    {
        if (facilityInfo.buildTime <= 0f)
        {
            // 建造时间为 0，直接完成
            CompleteBuilding();
            return;
        }

        buildTimer = facilityInfo.buildTime;
        SwitchState(State.Building);

        /*// 建造中显示预览贴图
        if (backgroundRenderer != null && facilityInfo.previewSprite != null)
            backgroundRenderer.sprite = facilityInfo.previewSprite;*/
    }

    private void Update()
    {
        if (currentState != State.Building) return;

        buildTimer -= Time.deltaTime;
        if (buildTimer <= 0f)
        {
            CompleteBuilding();
        }
    }

    /// <summary> 建造完成 </summary>
    private void CompleteBuilding()
    {
        buildTimer = 0f;
        SwitchState(State.Built);

        /*// 切换到建造完成贴图
        if (backgroundRenderer != null && facilityInfo.builtSprite != null)
            backgroundRenderer.sprite = facilityInfo.builtSprite;*/

        OnBuilt();
    }

    /// <summary> 建造完成回调，子类重写以启用设施功能 </summary>
    protected virtual void OnBuilt()
    {
        // 子类在此处激活攻击、生产等正常功能
    }

    private void SwitchState(State newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
