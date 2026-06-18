using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RectLineController : MonoBehaviour
{
    public static RectLineController instance { get; private set; }

    [SerializeField] private GameObject gameOver;

    private SpriteRenderer sr;

    [Header("检测范围")]
    public Vector2 size = new Vector2(1f, 0.2f); 
    public Vector2 offset = Vector2.zero;
    public LayerMask targetLayer;
    
    private BoxCollider2D hitbox;

    private float timer;
    private Color c; //虚线颜色
    public bool isBlink;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        c = sr.color;
    }

    void Update()
    {
        CheckBlink();
    }

    
    private void CheckBlink()
    {
        if (CheckAnimal())
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                sr.enabled = true;
                LineBink();
            }
        }
        else
        {
            isBlink = false;
            sr.enabled = false;

            c.a = 1f;
            sr.color = c;
            timer = .4f;
        }
    }

    public void EndGame()
    {
        gameOver.SetActive(true);
    }

    private void LineBink()
    {
        isBlink = true;

        float alpha = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(Time.time, 1f));
        c.a = alpha;
        sr.color = c;
    }

    public bool CheckAnimal()
    {
        Vector2 center = (Vector2)transform.position + offset;
        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, targetLayer);

        return hit != null; // 有碰撞体返回true，无则返回false
    }

    //判定箱，检测是否有动物接近虚线
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Vector3 drawPos = transform.position + (Vector3)offset;
        Gizmos.DrawCube(drawPos, size);
    }

    
}
