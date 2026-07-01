using UnityEngine;

/// <summary> 挂载在 UI 提示子物体上，激活时上下浮动 </summary>
public class FloatingUI : MonoBehaviour
{
    [Header("浮动参数")]
    [SerializeField] private float amplitude = 0.3f;
    [SerializeField] private float frequency = 2f;

    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * Mathf.PI * 2f * frequency) * amplitude;
        Vector3 pos = startPos;
        pos.y += offset;
        transform.localPosition = pos;
    }

    private void OnDisable()
    {
        transform.localPosition = startPos;
    }
}
