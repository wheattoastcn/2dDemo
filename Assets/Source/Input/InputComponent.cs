using UnityEngine;
using UnityEngine.InputSystem;

public class InputComponent : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookatAction;
    private InputAction lookAction;

    /// <summary> 当前帧的移动输入 (x: 左右, y: 上下) </summary>
    public Vector2 MoveInput { get; private set; }

    /// <summary> 当前帧是否按下了跳跃键 </summary>
    public bool JumpPressed { get; private set; }

    /// <summary> 当前帧是否松开了跳跃键 </summary>
    public bool JumpReleased { get; private set; }

    /// <summary> 跳跃键是否处于按住状态 </summary>
    public bool JumpHeld { get; private set; }

    /// <summary> 锁定镜头键是否在这一帧被按下 (Q键) </summary>
    public bool LookatPressed { get; private set; }

    /// <summary> 锁定镜头是否处于按住状态 (Q键 - 锁定镜头) </summary>
    public bool LookatHeld { get; private set; }

    /// <summary> 当前帧的鼠标屏幕坐标 </summary>
    public Vector2 MousePosition { get; private set; }

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
        lookatAction = playerMap.FindAction("LookAt");
        lookAction = playerMap.FindAction("Look");

        if (moveAction == null) Debug.LogError("InputComponent: 未找到 'Move' Action");
        if (jumpAction == null) Debug.LogError("InputComponent: 未找到 'Jump' Action");
        if (lookatAction == null) Debug.LogError("InputComponent: 未找到 'lookat' Action");
        if (lookAction == null) Debug.LogError("InputComponent: 未找到 'Look' Action");
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

        // 读取交互键状态 (Q键 - 锁定镜头)
        LookatPressed = lookatAction.WasPressedThisFrame();
        LookatHeld = lookatAction.IsPressed();

        // 从 Look Action 读取鼠标屏幕坐标 (Position [Mouse])
        MousePosition = lookAction.ReadValue<Vector2>();
    }
}
