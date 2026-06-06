using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FacilityPlacer : MonoBehaviour
{
    [Header("检测层")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask facilityLayer;

    [Header("地面")]
    [SerializeField] private float groundCheckDistance = 0.3f;

    [Header("颜色反馈")]
    [SerializeField] private Color validColor = new Color(0f, 1f, 0f, 0.5f);
    [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("输入")]
    [SerializeField] private InputComponent input;

    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    private bool isValidPlacement;

    /// <summary> 当前放置位置是否合法 </summary>
    public bool IsValid => isValidPlacement;

    private void Awake()
    {
        sr = transform.Find("Background").GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        boxCol.isTrigger = true;
        Physics2D.queriesStartInColliders = true;
    }

    /// <summary> 由 BuildManager 调用，注入数据和输入源 </summary>
    public void Setup(FacilityInfo info, InputComponent inputComp)
    {
        sr.sprite = info.previewSprite;
        boxCol.size = info.colliderSize;
        input = inputComp;
    }

    private void LateUpdate()
    {
        if (input == null) return;

        FollowMouse();
        CheckPlacement();
        UpdateColor();
    }

    /// <summary> 跟随鼠标，Y 锁定地面高度 </summary>
    private void FollowMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input.MousePosition);
        worldPos.z = 0f;
        //worldPos.y = groundY;
        transform.position = worldPos;
    }

    /// <summary> 碰撞检测：需要在地上 + 不与已有设施重叠 </summary>
    private void CheckPlacement()
    {
        // 射线从设施底部向下检测，小设施大设施自动适配
        float halfHeight = boxCol.size.y * 0.5f;
        Vector2 footOrigin = (Vector2)transform.position + Vector2.down * halfHeight;
        float checkDist = groundCheckDistance;

        // 1. 脚下是否有地面
        RaycastHit2D groundHit = Physics2D.Raycast(footOrigin, Vector2.down, checkDist, groundLayer);
        bool onGround = groundHit.collider != null;

        // 2. 设施身体是否插入地面（只检上方 80%，排除底部正常贴合）
        float bodyTopRatio = 0.8f;
        Vector2 bodyCheckSize = new Vector2(boxCol.size.x, boxCol.size.y * bodyTopRatio);
        Vector2 bodyCheckCenter = (Vector2)transform.position + Vector2.up * (boxCol.size.y * 0.1f);
        Collider2D bodyInGround = Physics2D.OverlapBox(bodyCheckCenter, bodyCheckSize, 0f, groundLayer);
        bool notInBodyGround = bodyInGround == null;

        // 3. 是否与已有设施重叠
        Collider2D overlap = Physics2D.OverlapBox(transform.position, boxCol.size, 0f, facilityLayer);
        bool noOverlap = overlap == null;

        isValidPlacement = onGround && notInBodyGround && noOverlap;

        Debug.DrawLine(footOrigin, footOrigin + Vector2.down * checkDist, onGround ? Color.green : Color.red);
        if (onGround)
            Debug.DrawLine(footOrigin, groundHit.point, Color.yellow);
    }

    /// <summary> 绿 / 红 半透明反馈 </summary>
    private void UpdateColor()
    {
        sr.color = isValidPlacement ? validColor : invalidColor;
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null) return;
        Gizmos.color = isValidPlacement ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, boxCol.size);
    }
}
