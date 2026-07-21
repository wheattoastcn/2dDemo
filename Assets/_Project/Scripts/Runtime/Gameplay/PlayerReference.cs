using UnityEngine;

/// <summary>
/// 挂载在 Player 上，用于跨场景注册。
/// 这样 UI 场景的 CameraController 等脚本可以通过 UIManager.Get&lt;PlayerReference&gt;() 拿到 Player
/// </summary>
public class PlayerReference : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        UIManager.Instance?.Unregister<PlayerReference>();
    }
}
