using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SlingshotController : MonoBehaviour
{
    public static SlingshotController instance { get; private set; }

    [SerializeField] private LayerMask hitLayer;

    [Header("动物预制体")]
    [SerializeField] private GameObject mouse;
    [SerializeField] private GameObject rabbit;
    [SerializeField] private GameObject sheep;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject tiger;
    [SerializeField] private GameObject loong;

    [Header("next动物预制体")]
    [SerializeField] private GameObject mouseNext;
    [SerializeField] private GameObject rabbitNext;
    [SerializeField] private GameObject sheepNext;

    private int thisAnimal;
    private int nextAnimal = 0;

    private GameObject animal;
    private CircleCollider2D animalCollider;
    private Rigidbody2D animalRB;

    [Header("皮筋")]
    private LineRenderer leftLineRenderer;
    private LineRenderer rightLineRenderer;
    private LineRenderer aimLine;
    private Transform leftPoint;
    private Transform rightPoint;
    private Transform centerPoint;

    Vector3 shootDir;
    Vector2 v0;
    private float shootDistance;

    private bool isDrawing = false;
    private bool haveAnimal = false;

    public float shootPower = 20f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        AnimalFactory.RegisterPrefab(AnimalType.mouse, mouse);
        AnimalFactory.RegisterPrefab(AnimalType.rabbit, rabbit);
        AnimalFactory.RegisterPrefab(AnimalType.sheep, sheep);
        AnimalFactory.RegisterPrefab(AnimalType.dog, dog);
        AnimalFactory.RegisterPrefab(AnimalType.tiger, tiger);
        AnimalFactory.RegisterPrefab(AnimalType.loong, loong);


        leftLineRenderer = transform.Find("LeftLine").GetComponent<LineRenderer>();
        rightLineRenderer = transform.Find("RightLine").GetComponent<LineRenderer>();
        aimLine = transform.Find("AimLine").GetComponent<LineRenderer>();

        leftPoint = transform.Find("LeftPoint");
        rightPoint = transform.Find("RightPoint");
        centerPoint = transform.Find("CenterPoint");

        //场景切换委托
        //GameManager.Instance.action_replay2 += Replay; GameManager.Instance.action_nextplay2 += Next_play;
    }

    private void Replay()
    {
        SceneManager.LoadScene(2);
    }

    private void Next_play()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (isDrawing)
        {
            Draw();
        }

        if (!haveAnimal)
            SetAnimal(transform.position);
    }

    public void StartDraw()
    {
        isDrawing = true;
        aimLine.enabled = true;
    }

    public void EndDraw()
    {
        isDrawing = false;
        aimLine.enabled = false;

        StartCoroutine(ElasticReturn(animal.transform.position));
    }

    //皮筋绘制
    public void Draw()
    {

        shootDir = (centerPoint.position - animal.transform.position).normalized;
        shootDistance = Vector2.Distance(animal.transform.position, centerPoint.position);
        v0 = shootDir * shootDistance * shootPower;

        Vector3 animalPosition = animal.transform.position;
        animalPosition = (animalPosition - centerPoint.position).normalized * animalCollider.radius * .7f + animalPosition;

        leftLineRenderer.SetPosition(0, animalPosition);
        leftLineRenderer.SetPosition(1, leftPoint.position);

        rightLineRenderer.SetPosition(0, animalPosition);
        rightLineRenderer.SetPosition(1, rightPoint.position);

    }

    //瞄准线控制
    public void SetAimLine(float _decayTime)
    {
        Vector3 start = animal.transform.position;
        Vector3 pos = start;

        Vector2 velocity = v0;
        Vector2 gravity = Physics2D.gravity;

        float dt = 0.02f;
        float vThreshold = 0.05f;

        List<Vector3> points = new List<Vector3>();
        points.Add(pos);

        bool hasReflected = false;
        int step = 0;
        int maxSteps = 500; // 安全上限
        float distanceTraveled = 0f;

        // 根据初速度估算最大模拟距离
        float speedFactor = 0.1f; // 可调节，控制瞄准线长度比例
        float maxSimDistance = velocity.magnitude * _decayTime * speedFactor;

        if (maxSimDistance > 6)
            maxSimDistance = 6;

        while (velocity.magnitude > vThreshold && step < maxSteps && distanceTraveled < maxSimDistance)
        {
            step++;
            Vector2 nextPos = (Vector2)pos + velocity * dt;

            // 计算本步位移
            float stepDistance = ((Vector2)nextPos - (Vector2)pos).magnitude;

            // 如果下一步超过最大长度，则直接将 pos 移动到最大长度
            if (distanceTraveled + stepDistance > maxSimDistance)
            {
                nextPos = (Vector2)pos + (nextPos - (Vector2)pos).normalized * (maxSimDistance - distanceTraveled);
                distanceTraveled = maxSimDistance;
            }
            else
            {
                distanceTraveled += stepDistance;
            }

            // 碰撞检测和反弹
            RaycastHit2D hit = Physics2D.Raycast(pos, velocity.normalized, (nextPos - (Vector2)pos).magnitude, hitLayer);
            if (hit.collider != null && !hasReflected)
            {
                points.Add(hit.point);
                velocity = Vector2.Reflect(velocity, hit.normal);
                pos = hit.point + velocity.normalized * 0.05f;
                hasReflected = true;
            }
            else
            {
                pos = nextPos;
            }

            // 衰减速度和重力
            velocity *= Mathf.Exp(-dt / _decayTime);
            velocity += gravity * dt * Mathf.Exp(-dt / _decayTime);

            points.Add(pos);
        }

        points.Add(pos); // 添加最终点

        // 设置 LineRenderer
        aimLine.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
            aimLine.SetPosition(i, points[i]);
    }

    public void SetAimLine2(float _decayTime)
    {
        List<Vector3> points = new List<Vector3>();

        Vector2 pos = animal.transform.position;
        Vector2 vel = v0;

        float animalSpeed = v0.magnitude;
        float animalGravity = animalRB.gravityScale;
        float animalDrag = animalRB.drag;

        float timeStep = 0.02f;
        float simulatedTime = 0f;

        for (int i = 0; i < 50;  i++)
        {
            points.Add(pos);

            simulatedTime += timeStep;
            float factor = Mathf.Exp(simulatedTime / _decayTime);

            vel *= factor;  
            vel += animalGravity * Physics2D.gravity * timeStep;
            pos += vel * timeStep;
        }

        aimLine.positionCount = points.Count;
        aimLine.SetPositions(points.ToArray());

    }



    #region 动物创建逻辑
    //生成动物
    public void SetAnimal(Vector3 _position)
    {
        haveAnimal = true;

        if (nextAnimal == 0)
            thisAnimal = Random.Range(1, 4);
        else
            thisAnimal = nextAnimal;


        if (thisAnimal == 1)
        {
            animal = Instantiate(mouse, Vector3.zero, Quaternion.identity);
        }

        else if (thisAnimal == 2)
        {
            animal = Instantiate(rabbit, Vector3.zero, Quaternion.identity);
        }

        else
        {
            animal = Instantiate(sheep, Vector3.zero, Quaternion.identity);
        }

        SetNextAnimal();

        animal.GetComponent<Animal>().isShooted = false;
        animal.transform.position = _position;
        animalCollider = animal.GetComponent<CircleCollider2D>();
        animalRB = animal.GetComponent<Rigidbody2D>();

        animalRB.isKinematic = true;
    }

    private void SetNextAnimal()
    {
        nextAnimal = Random.Range(1, 4);

        if (nextAnimal == 1)
        {
            mouseNext.SetActive(true);
            rabbitNext.SetActive(false);
            sheepNext.SetActive(false);
        }

        else if (nextAnimal == 2)
        {
            mouseNext.SetActive(false);
            rabbitNext.SetActive(true);
            sheepNext.SetActive(false);
        }

        else
        {
            mouseNext.SetActive(false);
            rabbitNext.SetActive(false);
            sheepNext.SetActive(true);
        }
    }

    private IEnumerator NewAnimal()
    {
        yield return new WaitForSeconds(.3f);
        haveAnimal = false;
    }
    #endregion

    public Vector3 getCenterPosition()
    {
        return centerPoint.transform.position;
    }
    
    //释放动物
    public void Shooting()
    {
        if (animal == null) return;

        animalRB.isKinematic = false;
        animal.GetComponent<Animal>().Launch(v0);

        StartCoroutine("NewAnimal");

    }

    //释放动物后皮筋回弹
    private IEnumerator ElasticReturn(Vector3 startPos)
    {
        float time = 0f;
        float duration = 0.2f; // 回弹时间，数值越小回弹越快

        while (time < duration)
        {
            time += Time.deltaTime;

            // 插值计算当前位置
            Vector3 pos = Vector3.Lerp(startPos, centerPoint.position, time / duration);

            leftLineRenderer.SetPosition(0, pos);
            rightLineRenderer.SetPosition(0, pos);

            yield return null;
        }

        //Draw();
    }

}
