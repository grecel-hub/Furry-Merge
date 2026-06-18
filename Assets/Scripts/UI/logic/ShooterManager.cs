using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    public RectTransform avatar;
    public RectTransform rangeCircle;
    [SerializeField] private float extraPadding = 10f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        UpdateCircleSize();

        Debug.Log("avatar.Scale: " + avatar.sizeDelta);
        Debug.Log("rangeCircle: " +  rangeCircle.sizeDelta);
    }

    void UpdateCircleSize()
    {
        float size = Mathf.Max(avatar.sizeDelta.x, avatar.sizeDelta.y) + extraPadding;
        rangeCircle.sizeDelta = new Vector2(size, size);
    }
}
