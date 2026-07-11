// UIPanel.cs
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]   // 便于做淡入淡出、可交互控制
public abstract class UIPanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // 显示面板（可被子类重写，加入播放动画等）
    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // 隐藏面板
    public virtual void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    // 当面板被打开时调用的初始化（比如刷新数据）
    public virtual void OnOpen(object data = null) { }

    // 当面板被关闭时调用
    public virtual void OnClose() { }
}