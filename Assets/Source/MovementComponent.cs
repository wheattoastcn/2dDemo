using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementComponent : MonoBehaviour
{
    [Header("移动")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("跳跃")]
    [SerializeField] private float jumpForce = 12f;

    [Header("地面检测")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.15f, 0.05f);
    [SerializeField] private LayerMask groundLayer = ~0;

    private Rigidbody2D rb;

    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary> 水平移动 </summary>
    public void Move(Vector2 input)
    {
        rb.linearVelocity = new Vector2(input.x * moveSpeed, rb.linearVelocity.y);
    }

    /// <summary> 当前水平速度 </summary>
    public float CurrentSpeedX => rb.linearVelocity.x;

    /// <summary> 跳跃 </summary>
    public void Jump()
    {
        if (!IsGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    /// <summary> 地面检测，建议在 Update 中调用 </summary>
    public void CheckGround()
    {
        if (groundCheckPoint == null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, groundLayer);
            IsGrounded = hit.collider != null;
        }
        else
        {
            IsGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0f, groundLayer);
        }
    }
}
