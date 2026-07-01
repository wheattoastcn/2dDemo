using System.Collections.Generic;
using UnityEngine;

// 单例模式的UI管理器，负责统筹所有UI的加载、显示与关闭
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Dictionary<string, BaseUI> _uiCache = new Dictionary<string, BaseUI>();
    private Transform _uiRoot; // 存放所有UI的父节点

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保证在场景切换时不被销毁
            _uiRoot = transform;
        }
        else Destroy(gameObject);
    }

    // 显示一个UI（泛型方法）
    public T ShowUI<T>() where T : BaseUI
    {
        string uiName = typeof(T).Name;
        if (_uiCache.TryGetValue(uiName, out BaseUI ui))
        {
            ui.Show();
            return ui as T;
        }
        // 如果没有找到，从Resources加载并实例化
        GameObject uiPrefab = Resources.Load<GameObject>($"UI/{uiName}");
        if (uiPrefab == null)
        {
            Debug.LogError($"未在 Resources/UI/ 路径下找到名为 {uiName} 的预制件！");
            return null;
        }
        GameObject uiObj = Instantiate(uiPrefab, _uiRoot);
        T newUI = uiObj.GetComponent<T>();
        if (newUI == null)
        {
            Debug.LogError($"预制件 {uiName} 上缺少 {uiName} 脚本组件！");
            return null;
        }
        _uiCache.Add(uiName, newUI);
        newUI.Show();
        return newUI;
    }
    // 关闭指定的UI
    public void CloseUI<T>() where T : BaseUI
    {
        string uiName = typeof(T).Name;
        if (_uiCache.ContainsKey(uiName))
        {
            _uiCache[uiName].Hide();
            // 如果UI不再常驻，可以将其销毁并从缓存中移除
            // Destroy(_uiCache[uiName].gameObject);
            // _uiCache.Remove(uiName);
        }
    }
}