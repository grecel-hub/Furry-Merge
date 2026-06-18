using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gravit : MonoBehaviour
{
    public static Gravit Instance;
    private void Awake()
    {
        Instance = this;
        list_fruit = new List<Fruit_controller>();
    }
    private GameObject gam_circle;private float dis_circle, dis_lose;
    [HideInInspector]public List<Fruit_controller> list_temp;
    public List<Fruit_controller> list_fruit;bool flash = false;
    [HideInInspector]public float outerBoundaryRadius;
    [HideInInspector]public bool game_end;private float wait_timer;private bool ready_end;
    private List<Fruit_controller> empty;
    private void Start()
    {
        empty = new List<Fruit_controller>();
        list_temp = new List<Fruit_controller>();
        gam_circle = GameObject.Find("redline");
        gam_circle.SetActive(false);
        dis_lose = gam_circle.GetComponent<SpriteRenderer>().bounds.extents.x;
        dis_circle = dis_lose + 0.5f;
        gam_circle.SetActive(false);
        Bounds bounds = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds;
        outerBoundaryRadius = Mathf.Min(bounds.extents.x, bounds.extents.y);
        GameManager.Instance.action_replay1+=Replay; GameManager.Instance.action_nextplay1 += Next_play;
    }
    private void Replay()
    {
        Time.timeScale = 1; game_end = false;
        foreach (Fruit_controller temp in list_temp)
        {
            if (temp != null)
                Destroy(temp.gameObject);
        }
            ;
        foreach (Fruit_controller temp in list_fruit)
        {
            if (temp != null)
                Destroy(temp.gameObject);
        }
        ;
        list_fruit.Clear(); list_temp.Clear();
        GetComponent<Controller>().Gam_again();
        Uimanager.Instance.End_game();
    }
    private void Next_play()
    {
        SceneManager.LoadScene(2);
    }
    void Update()
    {
        if (empty.Count > 0)
        {
            foreach (Fruit_controller temp in empty)
            {
                list_fruit.Remove(temp);
            }
            empty.Clear();
        }
        if (list_temp.Count > 0)
        {
            foreach(Fruit_controller temp in list_temp)
            {
                list_fruit.Add(temp);
            }
            list_temp.Clear();
        }
        if (list_fruit.Count > 0)
        {
            flash = false;ready_end = false;
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
                else
                {
                    empty.Add(melon);
                }
            }
            if (ready_end)
            {
                wait_timer += Time.deltaTime;
                if (wait_timer >= 1)
                {
                    GameManager.Instance .Show_end( Uimanager.Instance.score);
                    //Time.timeScale = 0; 
                    game_end = true;wait_timer = 0;
                }
            }
            else
            {
                wait_timer = 0;
            }
            if (flash)
            {
                gam_circle.SetActive(true);
            }
            else
            {
                gam_circle.SetActive(false);
            }
        }
    }

    void ApplyRadialGravity(Fruit_controller melon)
    {
        Rigidbody2D rb = melon.GetComponent<Rigidbody2D>();
        if (melon.impluce)
        { rb.AddForce(melon.dir * 10, ForceMode2D.Impulse); melon.impluce = false; }
        else
        {
            rb.AddForce(melon.dir * 20, ForceMode2D.Force);
        }
        melon.shake = false; 
    }

    void CheckOuterBoundary(Fruit_controller melon)
    {
        float distanceFromCenter = Vector2.Distance((Vector2)transform.position, melon.transform.position);
        if (melon.can_test&&( distanceFromCenter -melon.Get_radios < dis_circle))
        {
            if(melon.can_test)
            flash = true;
            if (distanceFromCenter - melon.Get_radios < dis_lose)
            {
                ready_end = true;
            }
        }
        if (distanceFromCenter + melon.Get_radios > outerBoundaryRadius)
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
