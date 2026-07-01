using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("目标")]
    [SerializeField] private Transform player;

    [Header("边缘触发")]
    [SerializeField][Range(0f, 200f)] private float edgeThreshold = 50f;
    [SerializeField] private float edgePanSpeed = 8f;

    [Header("回归")]
    [SerializeField] private float returnSpeed = 8f;

    [Header("垂直锁定")]
    [SerializeField] private float fixedY = 0f;

    private Camera cam;
    private InputComponent input;
    private bool isMovingToPlayer;
    private float moveStartX;
    private float moveTargetX;
    private float moveDuration;
    private float moveElapsed;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (player != null)
        {
            input = player.GetComponent<InputComponent>();

            Vector3 pos = transform.position;
            pos.y = fixedY;
            transform.position = pos;
        }
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Q键按下时触发线性移动
        if (input != null && input.LookatPressed && player != null)
        {
            StartMoveToPlayer();
        }

        // 线性移动到 Player
        if (isMovingToPlayer)
        {
            UpdateMoveToPlayer();
        }
        else
        {
            HandleEdgePan();
        }
    }

    private void StartMoveToPlayer()
    {
        isMovingToPlayer = true;
        moveStartX = transform.position.x;
        moveTargetX = player.position.x;
        float distance = Mathf.Abs(moveTargetX - moveStartX);
        moveDuration = distance / returnSpeed;
        moveElapsed = 0f;
    }

    private void UpdateMoveToPlayer()
    {
        if (moveDuration <= 0f)
        {
            SnapToTarget();
            return;
        }

        moveElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(moveElapsed / moveDuration);
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(moveStartX, moveTargetX, t);
        pos.y = fixedY;
        transform.position = pos;

        if (t >= 1f)
        {
            SnapToTarget();
        }
    }

    private void SnapToTarget()
    {
        Vector3 pos = transform.position;
        pos.x = moveTargetX;
        pos.y = fixedY;
        transform.position = pos;
        isMovingToPlayer = false;
    }

    /// <summary> 鼠标位于屏幕边缘时触发水平平移 </summary>
    private void HandleEdgePan()
    {
        if (input == null) return;

        Vector2 mousePos = input.MousePosition;
        float screenWidth = Screen.width;
        float direction = 0f;

        // 鼠标在左边缘
        if (mousePos.x <= edgeThreshold)
        {
            direction = -1f;
        }
        // 鼠标在右边缘
        else if (mousePos.x >= screenWidth - edgeThreshold)
        {
            direction = 1f;
        }

        if (Mathf.Approximately(direction, 0f)) return;

        Vector3 pos = transform.position;
        pos.x += direction * edgePanSpeed * Time.deltaTime;
        pos.y = fixedY;
        transform.position = pos;
    }
}
