using UnityEngine;

public class Fruit_controller : MonoBehaviour
{
    [Header("西瓜属性")]                
    public float mergeRadius = 0.5f;
    private GameObject gam;
    private Rigidbody2D rb;
    private bool isMerging = false;
    public  Fruit_curdata data;private Controller controller;
    private float radios;//半径
    [HideInInspector] public bool shake = false; [HideInInspector] public float timer;
    public float Get_radios { get { return radios; } }
    [HideInInspector] public Vector2 dir;
    public  void Initgam(Fruit_curdata op,bool shake=false)
    {
        gam = Resources.Load<GameObject>("prefab/gameobject/Circle");
        controller = FindAnyObjectByType<Controller>();
        rb = GetComponent<Rigidbody2D>();
        data = op;
        float scale = 1+ op.data.add_size * 0.5f;
        transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * scale;
        GetComponent<SpriteRenderer>().sprite = op.data.sprite;
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        radios = Mathf.Min(bounds.extents.x, bounds.extents.y);
        if (shake)
        {
            Vector2 dir = (transform.position - controller.transform.position).normalized;
            rb.AddForce(dir * 10);
            shake = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        shake = true;rb.velocity = Vector2.zero;dir = (transform.position - controller.transform .position).normalized;
        Fruit_controller otherMelon = collision.gameObject.GetComponent<Fruit_controller>();
        Fruit_controller thisMelon = GetComponent<Fruit_controller>();
        if(thisMelon.transform .position .y>=otherMelon.transform .position.y)
        {
            return;
        }
        if (otherMelon != null && thisMelon != null)
        {
            TryMergeWatermelons(thisMelon, otherMelon);
        }
    }

    void TryMergeWatermelons(Fruit_controller melon1, Fruit_controller melon2)
    {
        if (melon1.data.data.type == melon2.data.data.type)
        {
            // 防止重复合并
            melon1.isMerging = true;melon1.GetComponent<Collider2D>().enabled = false;
            melon2.isMerging = true;melon2.GetComponent<Collider2D>().enabled = false;
            // 计算中间位置
            Vector3 mergePosition = (melon1.transform.position + melon2.transform.position) * 0.5f;
            if (controller.Can_next(data.data.type))
            {
                Debug.Log("开始融合");
                Fruit_data temp_data = controller.Get_nexttype(data.data.type);
                Fruit_curdata temp = new Fruit_curdata(temp_data);
                SpawnMergedWatermelon(mergePosition, temp);
                Uimanager.Instance.Add_scores(data.data.score);
            }
            else
            {
                Uimanager.Instance.Add_scores(10);
            }
            // 销毁两个西瓜
            Destroy(melon1.gameObject);
            Destroy(melon2.gameObject);
        }
    }

    public void SpawnMergedWatermelon(Vector3 position, Fruit_curdata data)
    {
        Debug.Log("融合");
        GameObject newMelon = Instantiate(gam, position, Quaternion.identity);
        Fruit_controller watermelon = newMelon.GetComponent<Fruit_controller>();
        newMelon.GetComponent<Fruit_controller>().Initgam(data,true);
        Gravit.Instance.list_fruit.Add(newMelon.GetComponent<Fruit_controller>());
        newMelon.GetComponent<Fruit_controller>().shake = true;
    }
}