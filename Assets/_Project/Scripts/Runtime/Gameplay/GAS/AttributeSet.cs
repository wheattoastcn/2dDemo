using System;
using System.Collections.Generic;

/// <summary> PreAttributeChange 委托：允许修改即将生效的值 </summary>
public delegate void PreAttributeChangeDelegate(AttributeData data, ref float value);

/// <summary> 属性集，管理一组 AttributeData 的生命周期与修改流程 </summary>
public class AttributeSet
{
    private readonly Dictionary<string, AttributeData> attributes = new();

    /// <summary> 属性修改前回调：可在此修改即将生效的值 </summary>
    public event PreAttributeChangeDelegate PreAttributeChange;

    /// <summary> 属性修改后回调：变化已生效 </summary>
    public event Action<AttributeData, float, float> PostAttributeChange;

    /// <summary> 注册一个属性到集合中 </summary>
    public void RegisterAttribute(AttributeData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        attributes[data.Name] = data;

        // 挂内部监听，统一触发 PostAttributeChange
        data.OnBaseValueChanged += OnAttributeChanged;
    }

    /// <summary> 获取属性，若不存在返回 null </summary>
    public AttributeData GetAttribute(string name)
    {
        attributes.TryGetValue(name, out var data);
        return data;
    }

    /// <summary> 安全修改属性值，走 Pre → 修改 → Post 流程 </summary>
    public void SetBaseValue(string name, float desiredValue)
    {
        if (!attributes.TryGetValue(name, out var data))
            return;

        // PreChange：外部可修改 desiredValue
        float modified = desiredValue;
        PreAttributeChange?.Invoke(data, ref modified);

        // 实际写入
        data.SetBaseValue(modified);
    }

    private void OnAttributeChanged(AttributeData data, float oldValue, float newValue)
    {
        PostAttributeChange?.Invoke(data, oldValue, newValue);
    }
}
