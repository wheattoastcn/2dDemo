using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(MovementComponent))]
public class PlayerController : MonoBehaviour
{
    [Header("交互")]
    [SerializeField] private float interactRadius = 2f;
    [SerializeField] private LayerMask facilityLayer;

    private InputComponent input;
    private MovementComponent movement;
    private Facility nearestFacility;

    private void Awake()
    {
        input = GetComponent<InputComponent>();
        movement = GetComponent<MovementComponent>();
    }

    private void Update()
    {
        movement.CheckGround();
        HandleJump();
        HandleInteract();
    }

    private void FixedUpdate()
    {
        movement.Move(input.MoveInput);
        HandleFlip();
    }

    private void HandleFlip()
    {
        float dir = input.MoveInput.x;
        if (dir > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
        else if (dir < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1f);
    }

    private void HandleJump()
    {
        if (input.JumpPressed)
            movement.Jump();
    }

    /// <summary> 检测附近 PendingConstruction 设施，显示提示 + E键交互 </summary>
    private void HandleInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, facilityLayer);

        Facility found = null;
        float closestDist = float.MaxValue;
        foreach (var hit in hits)
        {
            Facility f = hit.GetComponent<Facility>();
            if (f == null || f.CurrentState != Facility.State.PendingConstruction)
                continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                found = f;
            }
        }

        // 离开上一个最近设施
        if (nearestFacility != null && nearestFacility != found)
            nearestFacility.HidePrompt();

        nearestFacility = found;

        // 进入新设施范围
        if (nearestFacility != null)
        {
            nearestFacility.ShowPrompt();

            if (input.InteractPressed)
                nearestFacility.Build();
        }
    }
}
