using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravit : MonoBehaviour
{
    public static Gravit Instance;
    private void Awake()
    {
        Instance = this;
        list_fruit = new List<Fruit_controller>();
    }
    public List<Fruit_controller> list_fruit;
    public float outerBoundaryRadius;
    private void Start()
    {
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        outerBoundaryRadius = Mathf.Min(bounds.extents.x, bounds.extents.y);
    }
    void FixedUpdate()
    {
        if (list_fruit.Count > 0)
        {
            foreach (Fruit_controller melon in list_fruit)
            {
                if (melon != null)
                {
                    if (melon.shake)
                    {
                        ApplyRadialGravity(melon);
                    }
                    CheckOuterBoundary(melon);
                }
            }
        }
    }

    void ApplyRadialGravity(Fruit_controller melon)
    {
        melon.timer += Time.deltaTime;
        if (melon.timer >= 0.3f)
        {
            melon.timer = 0; melon.shake = false;
        }
        Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();

        // 向外施加重力（离心力）
        rb.AddForce(melon.dir * 5);
    }

    void CheckOuterBoundary(Fruit_controller melon)
    {
        float distanceFromCenter = Vector2.Distance((Vector2)transform.position, melon.transform.position);
        distanceFromCenter += melon.Get_radios;
        if (distanceFromCenter > outerBoundaryRadius)
        {
            // 碰到外层圆，停止运动
            Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            Vector2 dir = (melon.transform.position - transform.position).normalized;
            // 确保西瓜不会超出边界
            Vector2 clampedPosition = dir * (outerBoundaryRadius - 0.001f) - dir * melon.Get_radios;
            melon.transform.position = clampedPosition;
        }
    }
}
