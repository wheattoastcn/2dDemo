using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputComponent))]
public class PlayerController : MonoBehaviour
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
    private InputComponent input;

    private bool isGrounded;
    private bool jumpPressedLastFrame;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<InputComponent>();
    }

    private void Update()
    {
        CheckGround();
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    /// <summary> 地面检测 </summary>
    private void CheckGround()
    {
        if (groundCheckPoint == null)
        {
            // 回退：用自身位置做射线检测
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.15f, groundLayer);
            isGrounded = hit.collider != null;
        }
        else
        {
            isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0f, groundLayer);
        }
    }

    /// <summary> 水平移动 </summary>
    private void HandleMove()
    {
        float moveDir = input.MoveInput.x;

        // 左右翻转
        if (moveDir > 0.01f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDir < -0.01f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // 设置水平速度，保留垂直速度
        rb.linearVelocity = new Vector2(moveDir * moveSpeed, rb.linearVelocity.y);
    }

    /// <summary> 跳跃（按下即跳，落地才能再跳） </summary>
    private void HandleJump()
    {
        if (input.JumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
