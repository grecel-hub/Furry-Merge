using UnityEngine;
using System.Collections;
public class Fruit_controller : MonoBehaviour
{
    [Header("西瓜属性")]
    public float mergeRadius = 0.5f;
    private GameObject gam;
    private Rigidbody2D rb;
    public bool shake;
    public Fruit_curdata data; public Controller controller;
    public float radios;//半径
     public bool can_test = false; LayerMask mask;
    public float Get_radios { get { return radios; } }
    private Vector2 Dir;
    [HideInInspector] public Vector2 dir { get { return (transform.position - controller.transform.position).normalized; } }
    private bool wait_destroy;[HideInInspector]public  bool impluce;
    private bool can_shake; [HideInInspector] public bool first_enter;
    [HideInInspector]public Effect_state effect_state;
    public void Initgam(Fruit_curdata op, Fruit_curdata next = null)
    {
        can_shake = true;mask = LayerMask.GetMask("Ignore Raycast");
        gam = Resources.Load<GameObject>("prefab/gameobject/Circle");
        controller = FindAnyObjectByType<Controller>();
        rb = GetComponent<Rigidbody2D>(); data = op; float scale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // 优化刚体参数
        rb.drag = 0.5f;
        rb.angularDrag = 4f;
        if (next != null)
        {
            scale = 1 + next.data.add_size * 0.2f;
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * scale;
        }
        else
        {
            scale = 1 + op.data.add_size * 0.2f;
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f) * scale;
            GetComponent<SpriteRenderer>().sprite = op.data.sprite;
            Bounds bounds = GetComponent<SpriteRenderer>().bounds;
            radios = Mathf.Min(bounds.extents.x, bounds.extents.y);
            // 根据水果大小调整质量
            rb.mass = Mathf.Clamp(transform.localScale.x, 0.5f, 3f);
        }
    }
    public void Set_destroy()
    {
        wait_destroy = true; GetComponent<Collider2D>().enabled = false;
    }
    public void Do_scale(Fruit_curdata op, Fruit_curdata next,Vector2 dir)
    {
        Initgam(op, next); float timer = 0;can_test = true;
        float scale = 1 + op.data.add_size * 0.3f; Vector3 temp = new Vector3(0.05f, 0.05f, 0.05f) * scale;
        rb.AddForce(dir * 10, ForceMode2D.Impulse);
        GetComponent<SpriteRenderer>().sprite = op.data.sprite;
        //StartCoroutine(Wait_radius());
        while (true)
        {
            timer += Time.deltaTime;
            transform.localScale = (timer / 0.5f) * temp;
            if (timer >= 0.5f) break;
        }
        transform.localScale = temp;
        // 根据水果大小调整质量
        rb.mass = Mathf.Clamp(transform.localScale.x, 0.5f, 3f);
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        radios = bounds.extents.x;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }
    Vector2 RotateWithQuaternion(Vector2 dir, float angle)
    {
        // 将Vector2转换为Vector3进行旋转
        Vector3 direction3D = new Vector3(dir.x, dir.y, 0);

        // 使用Quaternion进行旋转
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Vector3 rotated = rotation * direction3D;

        return new Vector2(rotated.x, rotated.y).normalized;
    }

    private void Update()
    {
        if (can_test)
        {
            Vector2 dir = (transform.position - controller.transform.position).normalized;
            Vector2 dir1 =dir;
            Vector2 dir2 = RotateWithQuaternion(dir, 90);
            Vector2 dir3 = RotateWithQuaternion(dir, -90);
            Vector2 dir4 = RotateWithQuaternion(dir,45);
            Vector2 dir5 = RotateWithQuaternion(dir, -45);
            Vector2 origin1 = (Vector2)transform.position + dir1 * (radios+0.01f);
            Vector2 origin2 = (Vector2)transform.position + dir2 * (radios + 0.01f);
            Vector2 origin3 = (Vector2)transform.position + dir3 * (radios + 0.01f);
            Vector2 origin4 = (Vector2)transform.position + dir4 * (radios + 0.01f);
            Vector2 origin5 = (Vector2)transform.position + dir5 * (radios + 0.01f);
            if (Vector2.Distance(origin1,controller.transform .position) > controller.range_dis)
            {
                return;
            }
            RaycastHit2D hit1 = Physics2D.Raycast(origin1, dir1,0.05f,mask );
            RaycastHit2D hit2 = Physics2D.Raycast(origin2, dir2,0.05f,mask );
            RaycastHit2D hit3 = Physics2D.Raycast(origin3, dir3,0.05f,mask );
            RaycastHit2D hit4 = Physics2D.Raycast(origin4, dir4, 0.05f,mask);
            RaycastHit2D hit5 = Physics2D.Raycast(origin5, dir5, 0.05f,mask);
            Debug.DrawLine(origin1, origin1+dir1*0.05f , Color.black);
            Debug.DrawLine(origin2, origin2 + dir2*0.05f , Color.black);
            Debug.DrawLine(origin3, origin3 + dir3*0.05f , Color.black);
            Debug.DrawLine(origin4, origin4 + dir4 * 0.05f, Color.black);
            Debug.DrawLine(origin5, origin5 + dir5 * 0.05f, Color.black);
            if (hit1.collider==null&& hit2.collider == null && hit4.collider == null)
            {
                if (can_shake)
                {
                    shake = true; StartCoroutine(Wait_shake()); can_shake = false;
                    Debug.Log("检测为空"); first_enter = true;return;
                }
            }
            if (hit1.collider == null && hit3.collider == null && hit5.collider == null)
            {
                if (can_shake)
                {
                    shake = true; StartCoroutine(Wait_shake()); can_shake = false;
                    Debug.Log("检测为空"); first_enter = true;
                }
            }
        }
    }
    private IEnumerator Wait_shake()
    {
        yield return new WaitForSeconds(0.1f);
        can_shake = true; rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (wait_destroy) return;
        if (collision == null) return;
        rb.velocity = Vector2.zero;
        if (data.data.type != collision.gameObject.GetComponent<Fruit_controller>().data.data.type)
        {
            if (rb.velocity.magnitude < 0.05f && collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < 0.05f)
            {
                rb.velocity = Vector2.zero; return;
            }
        }
        if (can_shake)
        {
            shake = true; 
            StartCoroutine(Wait_shake()); can_shake = false;
        }
        if (can_test == false)
        {
            can_test = true; first_enter = true;collision.gameObject.GetComponent<Fruit_controller>().impluce = true;
        }
        else
        {
            first_enter = false;
        }
        Fruit_controller otherMelon = collision.gameObject.GetComponent<Fruit_controller>();
        Fruit_controller thisMelon = GetComponent<Fruit_controller>();
        if (thisMelon.transform.position.y < otherMelon.transform.position.y)
        {
            return;
        }
        else if (thisMelon.transform.position.y == otherMelon.transform.position.y)
        {
            if (thisMelon.transform.position.x > otherMelon.transform.position.x)
            {
                return;
            }
        }
        if (otherMelon != null && thisMelon != null)
        {
            TryMergeWatermelons(thisMelon, otherMelon);
        }

    }
    private IEnumerator Wait_mode()
    {
        yield return null;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }
    void TryMergeWatermelons(Fruit_controller melon1, Fruit_controller melon2)
    {
        if (melon1.data.data.type == melon2.data.data.type)
        {
            // 防止重复合并
            melon1.GetComponent<Fruit_controller>().Set_destroy();
            melon2.GetComponent<Fruit_controller>().Set_destroy();
            // 计算中间位置
            float dis1 = Vector3.Distance(melon1.transform.position, controller.gameObject.transform.position);
            float dis2 = Vector3.Distance(melon2.transform.position, controller.gameObject.transform.position);
            Vector3 mergePosition;
            Vector2 dir;
            if (dis1 > dis2)
            {
                mergePosition = melon2.transform.position;
                melon2.GetComponent<SpriteRenderer>().enabled = false;
                dir = (melon1.transform.position - melon2.transform.position).normalized;
            }
            else
            {
                mergePosition = melon1.transform.position;
                melon1.GetComponent<SpriteRenderer>().enabled = false;
                dir = (melon2.transform.position - melon1.transform.position).normalized;
            }
            if (controller.Can_next(data.data.type))
            {
                Debug.Log("开始融合");
                Fruit_data temp_data = controller.Get_nexttype(data.data.type);
                Fruit_curdata temp = new Fruit_curdata(temp_data);
                SpawnMergedWatermelon(mergePosition, temp,dir);
                Uimanager.Instance.Add_scores(data.data.score);
            }
            else
            {
                Uimanager.Instance.Add_scores(data.data.score);
            }
            // 销毁两个西瓜
            StartCoroutine(Wait_destroy(melon1.gameObject));
            StartCoroutine(Wait_destroy(melon2.gameObject));
        }
    }
    private IEnumerator Wait_destroy(GameObject temp)
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(temp);
    }

    public void SpawnMergedWatermelon(Vector3 position, Fruit_curdata data,Vector2 dir)
    {
        Debug.Log("融合");
        GameObject newMelon = Instantiate(gam, position, Quaternion.identity);
        newMelon.GetComponent<Fruit_controller>().effect_state= EffectManager.instance.PlayEffect(position,data.data.type,newMelon.transform);
        Fruit_controller watermelon = newMelon.GetComponent<Fruit_controller>();
        newMelon.GetComponent<Fruit_controller>().Do_scale(data, new Fruit_curdata(controller.GetFruitData(data.data.type)),dir);
        Gravit.Instance.list_temp.Add(newMelon.GetComponent<Fruit_controller>());
    }
    private void OnDestroy()
    {
        if (effect_state != null)
        {
            EffectManager.instance.Add_(effect_state);
        }
    }
}