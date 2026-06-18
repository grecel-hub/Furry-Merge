using UnityEngine;

public class Animator_test : MonoBehaviour
{
    private GameObject gam_txt;private bool first_enter;private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        gam_txt = GameObject.Find("Canvas_main/txt").gameObject;first_enter = false;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!first_enter)
        {
            if (Input.touchCount > 0||Input.GetMouseButtonDown(0))
            {
                gam_txt.SetActive(false);first_enter = true;
                animator.SetBool("show", true);
            }
        }
    }
}
