using UnityEngine;
using UnityEngine.UI;

// 为UI窗口的生命周期定义标准
public abstract class BaseUI : MonoBehaviour
{
    // 供外部引用的公共接口
    public void Show() { gameObject.SetActive(true); OnShow(); }
    public void Hide() { gameObject.SetActive(false); OnHide(); }

    protected virtual void Awake() { /* 初始化组件引用等 */ }
    protected virtual void Start() { /* 初始化数据等 */ }
    protected virtual void OnShow() { /* 每次显示时的逻辑，如刷新数据 */ }
    protected virtual void OnHide() { /* 每次隐藏时的逻辑 */ }

    // 供子类复用的绑定UI事件方法
    protected void BindButtonEvent(Button button, System.Action onClickAction)
    {
        if (button != null) button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}
