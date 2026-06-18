using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Animal : MonoBehaviour
{
    [SerializeField] private AnimalType type;

    [Header("虚线检测")]
    private bool canCheckRectLine;
    [SerializeField] private Transform checkRectLine;
    [SerializeField] private Vector3 bottomOffset;
    [SerializeField] private LayerMask rectLineLayer;


    private Animal animal;
    private Animal collisionAnimal;
    private SlingshotController slingshotController = SlingshotController.instance;

    private Rigidbody2D rb;
    private CircleCollider2D cd;

    public float maxDistance = 1f;

    [Header("发射参数")]
    private float launchTime;
    private bool isLaunched;
    private float decayTime = 5f; //衰减系数

    public bool isShooted = true;

    private bool isMouseDown = false;
    private bool isMerged = false; //合成锁

    private Effect_state effect_state;
    private void Start()
    {
        animal = GetComponent<Animal>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        CheckGameOver();

        if (!isShooted)
        {
            MoveControl();
        }

        if (isMerged)
            StartCoroutine(DestroyAfterFrame(.25f));

    }

    private void CheckGameOver()
    {
        float radius = cd.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);

        bottomOffset = new Vector3(0, -radius, 0);
        checkRectLine.position = transform.position + bottomOffset;
        //Debug.Log(checkRectLine.position);

        if (canCheckRectLine && checkRectLine.position.y > RectLineController.instance.transform.position.y)
        {
            RectLineController.instance.EndGame();
        }
    }

    #region 运动逻辑
    public void Launch(Vector2 v0)
    {
        rb.isKinematic = false;
        rb.velocity = v0;
        launchTime = Time.time;
        isLaunched = true;
    }

    private void FixedUpdate()
    {
        if (isLaunched)
        {
            float t = Time.time - launchTime; 
            float factor = Mathf.Exp(-t / decayTime);
            rb.velocity = rb.velocity * factor;

            float gravityY = Physics2D.gravity.y * rb.gravityScale * Time.fixedDeltaTime;

            if (rb.velocity.magnitude <= Mathf.Abs(gravityY) * 2f)
            {
                //rb.velocity = Vector2.zero;
                isLaunched = false;
               
            }
        }
    }
    #endregion

    #region 鼠标操控
    private void OnMouseDown()
    {
        if (!isShooted)
        {
            isMouseDown = true;
            MoveControl();
            slingshotController.StartDraw();
        }

    }

    private void OnMouseUp()
    {
        if(!isShooted && isMouseDown)
        { 
            SFXManager.instance.shotPlay();

            isMouseDown = false;

            slingshotController.EndDraw();
            slingshotController.Shooting();

            isShooted = true;

            StartCoroutine("StartCheckRectLine");
        }
    }

    private void MoveControl()
    {
        if (isMouseDown)
        {
            transform.position = GetMousePosition();
        }
    }
#endregion

    //弹弓活动范围限制
    private Vector3 GetMousePosition() 
    {
        Vector3 centerPosition = SlingshotController.instance.getCenterPosition();
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mp.z = 0;

        Vector3 dir = mp - centerPosition;

        Vector3 forwardDir = Vector3.up;

        float angle = Vector3.SignedAngle(forwardDir, dir, Vector3.forward);

        //鼠标向下移动0.2时取消释放
        if (dir.y < -0.4f)
        {
            isMouseDown = false;
            return transform.position;
        }

        slingshotController.SetAimLine(decayTime);

        float maxAngle = 60f;
        angle = Mathf.Clamp(angle, -maxAngle, maxAngle);

        Vector3 clampedDir = Quaternion.Euler(0, 0, angle) * forwardDir.normalized;

        float distance = Mathf.Min(dir.magnitude, maxDistance);
        mp = centerPosition + clampedDir * distance;

        return mp;
    }

    //合体同等级动物并进化
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionAnimal = collision.gameObject.GetComponent<Animal>();
        
        if (collisionAnimal != null && animal != null)
        {
            if (collisionAnimal.type == animal.type && !isMerged && !collisionAnimal.isMerged && animal.isShooted && collisionAnimal.isShooted)
            {
                isMerged = true;
                collisionAnimal.isMerged = true;

                AnimalType newAnimalType = AnimalFactory.GetNextEvolution(animal.type);

                ScoreManager.instance.AddScore(animal.type);

                // 生成新动物逻辑，newAnimalType == loong则只销毁不生成
                if (newAnimalType != animal.type)
                {
                    StartCoroutine(AnimalMoveAndEvolution(transform.position, collision.transform.position, newAnimalType));
                }
                
            }
        }
    }

    private IEnumerator AnimalMoveAndEvolution(Vector3 startPos, Vector3 targetPos, AnimalType newAnimalType)
    {
        float duration = .1f;
        Fruittype type = (Fruittype)((int)newAnimalType);
        effect_state=EffectManager.instance.PlayEffect(transform.position ,type,transform);

        collisionAnimal.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        rb.bodyType = RigidbodyType2D.Static;
        collisionAnimal.GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        

        yield return StartCoroutine(MoveAnimal(startPos, targetPos, duration));
        yield return StartCoroutine(AnimalEvolution(evolutionPos(startPos, targetPos), newAnimalType, duration));

    }

    private Vector3 evolutionPos(Vector3 startPos, Vector3 targetPos)
    {
        if (startPos.y < targetPos.y)
            return startPos;
        else
            return targetPos;
    }

    //回收
    private void OnDestroy()
    {
        if(effect_state!=null)
        EffectManager.instance.Add_(effect_state);
    }

    private IEnumerator DestroyAfterFrame(float seconds)
    {
        if (seconds != 0)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(this.gameObject);
        }

        else
        {
            yield return null; // 等待一帧，让协程安全结束
            Destroy(this.gameObject);
        }
    }

    private IEnumerator MoveAnimal(Vector3 _startPos, Vector3 _targetPos, float _duration)
    {
        float elapsed = 0;

        while (elapsed < _duration)
        {

            float t = elapsed / _duration;
            transform.position = Vector3.Lerp(_startPos, _targetPos, t);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        transform.position = _targetPos;
    }

    private IEnumerator AnimalEvolution(Vector3 _targetPos, AnimalType _newAnimalType, float _duration)
    {
        float elapsed = 0;
        GameObject newAnimal = AnimalFactory.CreateAnimal(_newAnimalType, _targetPos, Quaternion.identity);
        Vector3 scale = newAnimal.transform.localScale;
        newAnimal.transform.localScale = scale * 0.5f;

        while (elapsed < _duration)
        {

            float t = elapsed / _duration;
            newAnimal.transform.localScale = Vector3.Lerp(scale * 0.5f, scale, t);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        newAnimal.transform.localScale = scale;
    }

    private IEnumerator StartCheckRectLine()
    {
        yield return new WaitForSeconds(.3f);
        canCheckRectLine = true;
    }

}
