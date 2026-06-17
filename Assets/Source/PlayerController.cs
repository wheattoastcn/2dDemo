using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(MovementComponent))]
public class PlayerController : MonoBehaviour
{
    private InputComponent input;
    private MovementComponent movement;

    private void Awake()
    {
        input = GetComponent<InputComponent>();
        movement = GetComponent<MovementComponent>();
    }

    private void Update()
    {
        movement.CheckGround();
        HandleJump();
    }

    private void FixedUpdate()
    {
        movement.Move(input.MoveInput);
        HandleFlip();
    }

    /// <summary> 左右翻转 </summary>
    private void HandleFlip()
    {
        float dir = input.MoveInput.x;
        if (dir > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
        else if (dir < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
    }

    /// <summary> 跳跃 </summary>
    private void HandleJump()
    {
        if (input.JumpPressed)
            movement.Jump();
    }
}
