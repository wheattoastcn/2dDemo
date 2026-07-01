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

    private SpriteRenderer displaySr;    // Display 子物体的 SpriteRenderer，显示设施本体
    private SpriteRenderer overlaySr;    // Background 子物体的 SpriteRenderer，只负责变色
    private BoxCollider2D boxCol;
    private bool isValidPlacement;

    public bool IsValid => isValidPlacement;

    /// <summary> 用自身 groundLayer 做射线，返回贴地坐标 </summary>
    public Vector3 SnapToGround(Vector3 pos, float halfHeight)
    {
        Vector2 origin = (Vector2)pos + Vector2.down * halfHeight;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 10f, groundLayer);
        if (hit.collider != null)
            pos.y = hit.point.y + halfHeight;
        return pos;
    }

    private void Awake()
    {
        displaySr = transform.Find("Display").GetComponent<SpriteRenderer>();
        overlaySr = transform.Find("Background").GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
        boxCol.isTrigger = true;
        Physics2D.queriesStartInColliders = true;
    }

    /// <summary> 由 BuildManager 调用，注入数据和输入源 </summary>
    public void Setup(FacilityInfo info, InputComponent inputComp)
    {
        // 从 Prefab 的子对象 "Sprite" 上取精灵和缩放
        var prefabSprite = info.prefab.transform.Find("Sprite");
        var prefabSr = prefabSprite.GetComponent<SpriteRenderer>();
        if (prefabSr != null)
        {
            displaySr.sprite = prefabSr.sprite;
            displaySr.transform.localScale = prefabSprite.localScale;
        }

        boxCol.size = info.colliderSize;
        overlaySr.transform.localScale = info.colliderSize;
        input = inputComp;
    }

    private void LateUpdate()
    {
        if (input == null) return;

        FollowMouse();
        CheckPlacement();
        UpdateOverlay();
    }

    private void FollowMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input.MousePosition);
        worldPos.z = 0f;
        transform.position = worldPos;
    }

    private void CheckPlacement()
    {
        float halfHeight = boxCol.size.y * 0.5f;
        Vector2 footOrigin = (Vector2)transform.position + Vector2.down * halfHeight;
        float checkDist = groundCheckDistance;

        RaycastHit2D groundHit = Physics2D.Raycast(footOrigin, Vector2.down, checkDist, groundLayer);
        bool onGround = groundHit.collider != null;

        float bodyTopRatio = 0.95f;
        Vector2 bodyCheckSize = new Vector2(boxCol.size.x, boxCol.size.y * bodyTopRatio);
        Vector2 bodyCheckCenter = (Vector2)transform.position + Vector2.up * (boxCol.size.y * 0.025f);
        Collider2D bodyInGround = Physics2D.OverlapBox(bodyCheckCenter, bodyCheckSize, 0f, groundLayer);
        bool notInBodyGround = bodyInGround == null;

        Collider2D overlap = Physics2D.OverlapBox(transform.position, boxCol.size, 0f, facilityLayer);
        bool noOverlap = overlap == null;

        isValidPlacement = onGround && notInBodyGround && noOverlap;

        Debug.DrawLine(footOrigin, footOrigin + Vector2.down * checkDist, onGround ? Color.green : Color.red);
        if (onGround)
            Debug.DrawLine(footOrigin, groundHit.point, Color.yellow);
    }

    /// <summary> Background 子物体覆盖层变色，设施本体保持原色 </summary>
    private void UpdateOverlay()
    {
        if (overlaySr != null)
            overlaySr.color = isValidPlacement ? validColor : invalidColor;
    }

    private void OnDrawGizmos()
    {
        if (boxCol == null) return;
        Gizmos.color = isValidPlacement ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, boxCol.size);
    }
}
