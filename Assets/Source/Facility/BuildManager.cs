using System;
using UnityEngine;

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

    private FacilityInfo currentFacility;
    private FacilityPlacer currentPlacer;

    private void Update()
    {
        if (CurrentState != State.Previewing || currentPlacer == null) return;

        // 右键 / ESC 取消
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPreview();
            return;
        }

        // 左键确认：绿色 + 按下
        if (currentPlacer.IsValid && Input.GetMouseButtonDown(0))
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

    /// <summary> 绿色状态下确认，实例化设施 </summary>
    private void PlaceFacility()
    {
        GameObject facilityObj = Instantiate(currentFacility.prefab,
            currentPlacer.transform.position,
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
