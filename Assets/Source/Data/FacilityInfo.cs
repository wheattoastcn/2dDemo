using UnityEngine;

/// <summary> 设施纯数据结构体，不含 Unity 生命周期 </summary>
[System.Serializable]
public struct FacilityInfo
{
    public string facilityName;
    public GameObject prefab;
    public Sprite previewSprite;
    public Sprite builtSprite;
    public Vector2 colliderSize;
    public float buildTime;
    public int cost;
}
