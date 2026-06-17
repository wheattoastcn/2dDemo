using System;

/// <summary> 属性数据，持有基础值与变化事件 </summary>
[Serializable]
public class AttributeData
{
    public string Name { get; private set; }
    public float BaseValue { get; private set; }

    /// <summary> 属性值变化委托：AttributeData自身, 旧值, 新值 </summary>
    public event Action<AttributeData, float, float> OnBaseValueChanged;

    public AttributeData(string name, float baseValue)
    {
        Name = name;
        BaseValue = baseValue;
    }

    public void SetBaseValue(float newValue)
    {
        if (Math.Abs(BaseValue - newValue) < float.Epsilon) return;

        float oldValue = BaseValue;
        BaseValue = newValue;
        OnBaseValueChanged?.Invoke(this, oldValue, newValue);
    }
}
