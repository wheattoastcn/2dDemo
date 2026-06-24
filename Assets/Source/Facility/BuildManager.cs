using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public enum State { Idle, Previewing }

    [Header("配置")]
    [SerializeField] private facilityData facilityDatas;
    [SerializeField] private FacilityPlacer placerPrefab;
    [SerializeField] private Transform facilityParent;

    [Header("输入")]
    [SerializeField] private InputComponent input;

    /// <summary> 当前状态 </summary>
    public State CurrentState { get; private set; } = State.Idle;

    /// <summary> 状态变化事件（UI 等外部监听） </summary>
    public event Action<State> OnStateChanged;

    /// <summary> 设施列表（供 UI 动态生成按钮） </summary>
    public System.Collections.Generic.List<FacilityInfo> FacilityList => facilityDatas.facilityDatas;

    private FacilityInfo currentFacility;
    private FacilityPlacer currentPlacer;

    private void Update()
    {
        if (CurrentState != State.Previewing || currentPlacer == null) return;

        // ESC 取消
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelPreview();
            return;
        }

        // 确认键（Attack Action）：绿色 + 按下
        if (currentPlacer.IsValid && input.AttackPressed)
        {
            PlaceFacility();
        }
    }

    /// <summary> 由 UI 调用，选择设施进入预览 </summary>
    public void SelectFacility(string facilityName)
    {
        if (CurrentState == State.Previewing)
            CancelPreview();

        FacilityInfo info = facilityDatas.GetFacilityInfo(facilityName);

        if (string.IsNullOrEmpty(info.facilityName))
        {
            Debug.LogWarning($"BuildManager: 未找到设施 '{facilityName}'");
            return;
        }

        currentFacility = info;
        currentPlacer = Instantiate(placerPrefab);
        currentPlacer.Setup(info, input);

        SwitchState(State.Previewing);
    }

    /// <summary> 绿色状态下确认，实例化设施并开始建造 </summary>
    private void PlaceFacility()
    {
        Vector3 pos = currentPlacer.SnapToGround(currentPlacer.transform.position,
            currentFacility.colliderSize.y * 0.5f);

        GameObject facilityObj = Instantiate(currentFacility.prefab,
            pos,
            Quaternion.identity,
            facilityParent);

        // 设为 Facility Layer，后续 Placer 的 OverlapBox 可检测到
        facilityObj.layer = LayerMask.NameToLayer("Facility");

        CancelPreview();
    }

    /// <summary> 取消预览，销毁 Placer </summary>
    private void CancelPreview()
    {
        if (currentPlacer != null)
        {
            Destroy(currentPlacer.gameObject);
            currentPlacer = null;
        }

        currentFacility = default;
        SwitchState(State.Idle);
    }

    private void SwitchState(State newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}
