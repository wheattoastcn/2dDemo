using UnityEngine;
using UnityEngine.InputSystem;

public class InputComponent : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;

    /// <summary> 当前帧的移动输入 (x: 左右, y: 上下) </summary>
    public Vector2 MoveInput { get; private set; }

    /// <summary> 当前帧是否按下了跳跃键 </summary>
    public bool JumpPressed { get; private set; }

    /// <summary> 当前帧是否松开了跳跃键 </summary>
    public bool JumpReleased { get; private set; }

    /// <summary> 跳跃键是否处于按住状态 </summary>
    public bool JumpHeld { get; private set; }

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputComponent: 未设置 InputActionAsset，请在 Inspector 中拖入 InputSystem_Actions");
            return;
        }

        // 查找 Player Action Map
        var playerMap = inputActions.FindActionMap("Player");
        if (playerMap == null)
        {
            Debug.LogError("InputComponent: 未找到 'Player' Action Map");
            return;
        }

        moveAction = playerMap.FindAction("Move");
        jumpAction = playerMap.FindAction("Jump");

        if (moveAction == null) Debug.LogError("InputComponent: 未找到 'Move' Action");
        if (jumpAction == null) Debug.LogError("InputComponent: 未找到 'Jump' Action");
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Enable();
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null || jumpAction == null) return;

        // 读取移动输入
        MoveInput = moveAction.ReadValue<Vector2>();

        // 读取跳跃按钮状态 (新版 Input System 1.0+ 使用 WasPressedThisFrame / WasReleasedThisFrame)
        JumpPressed = jumpAction.WasPressedThisFrame();
        JumpReleased = jumpAction.WasReleasedThisFrame();
        JumpHeld = jumpAction.IsPressed();
    }
}
