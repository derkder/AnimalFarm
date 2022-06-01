using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement instance;
    public static PlayerMovement Instance
    {
        get
        {
            if (null == instance)
            {
                GameObject go = GameObject.Find("Characters/Dino");
                if (null != go)
                {
                    instance = go.GetComponent<PlayerMovement>();
                }
                else
                {
                    Debug.Log("can not find GameObject.farmer");
                }
            }
            return instance; 
        }
    }
    public event Action<GameObject> OnAttack;
    public event Action<GameObject> OnSkill;
    public bool testInvoke = false;
    public float speed;
    new private Rigidbody2D rigidbody;
    private Collider2D coll;
    private Animator animator;
    private float inputX, inputY;
    public float stopX, stopY;
    public bool isMove=false;
    private float preinputX = 1;
    private Vector3 rightVec;
    private float skillTime = 0.4f;//技能动画播放的时长也是技能对应协程的时长
    private float hitTime = 0.4f;//收到攻击后的硬直
    public int hp = 6;
    public GameObject smallBroken;
    public GameObject bigBroken;
    //private AnimatorStateInfo info;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rightVec = transform.right;
    }

    void Update()
    {
        SkillRelease();
        AttackRelease();
        //PureForTest();
    }
    private void FixedUpdate()//把三段攻击的逻辑放在fsm里了
    {
        GroundMovement();
    }
    void GroundMovement()//移动
    {
        inputX = Input.GetAxisRaw("Horizontal");//只返回-1，0，1
        inputY = Input.GetAxisRaw("Vertical");
        Vector2 input = (rightVec * inputX + transform.up * inputY).normalized;
        rigidbody.velocity = input * speed;
        if (input != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            stopX = inputX;
            stopY = inputY;
            if (inputX != 0 && inputX != preinputX)
            {
                transform.Rotate(0f, 180f, 0f);
                preinputX = inputX;
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        animator.SetFloat("InputX", stopX);
        animator.SetFloat("InputY", stopY);
    }
    //三个技能
    #region
    void SkillRelease()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SkillRoar"))
            {
                GameObject Dino = GameObject.Find("Characters/Dino");
                OnSkill?.Invoke(Dino);
            }
            animator.SetBool("SkillRoar", true);
            StartCoroutine(SkillRoar());
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SkillFire"))
            {
                GameObject Dino = GameObject.Find("Characters/Dino");
                OnSkill?.Invoke(Dino);
            }
            animator.SetBool("SkillFire", true); 
            StartCoroutine(SkillFire());
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (!gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SkillAk"))
            {
                GameObject Dino = GameObject.Find("Characters/Dino");
                OnSkill?.Invoke(Dino);
            }
            animator.SetBool("SkillAk", true);
            StartCoroutine(SkillAk());
        }
    }
    IEnumerator SkillRoar()
    {
        yield return new WaitForSecondsRealtime(skillTime);
        animator.SetBool("SkillRoar", false);
    }
    IEnumerator SkillFire()
    {
        yield return new WaitForSecondsRealtime(skillTime);
        animator.SetBool("SkillFire", false);
    }
    IEnumerator SkillAk()
    {
        yield return new WaitForSecondsRealtime(skillTime);
        animator.SetBool("SkillAk", false);
    }
    #endregion

    //攻击传递
    #region
    void AttackRelease()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_boxing") ||
                gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_kick")|| 
                gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attack_tail"))
            {
                testInvoke = true;
                GameObject Dino = GameObject.Find("Characters/Dino");
                OnAttack?.Invoke(Dino);//传递是谁打的
            }
        }
    }
    #endregion
    public void Injured()//被农夫或boss打倒后被调用
    {
        animator.SetBool("isHit", true);
        StartCoroutine(Hited());
    }
    IEnumerator Hited()
    {
        yield return new WaitForSecondsRealtime(hitTime);
        animator.SetBool("isHit", false);
        hp--;
    }
    void OnCollisionEnter2D(Collision2D collision)//一直贴着也只会触发一次
    {
        Debug.Log("OnCollisionEnter2D");
        if (collision.gameObject.tag=="fence"&&Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("enterOnCollisionEnter2D");
            GameManager.addScore(2);
            Transform temp = collision.gameObject.transform;
            Destroy(collision.gameObject);
            Debug.Log("InstantiatesmallBroken");
            Vector3 explore = temp.position;
            explore.x += 5;
            explore.y += 5;
            Instantiate(smallBroken, explore, temp.rotation);
            //在这个位置intanciate一个挂着爆炸动画的prefab，播放完后destroy
        }
        else if(collision.gameObject.tag == "house" && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("enterOnCollisionEnter2D");
            GameManager.addScore(8);
            Transform temp = collision.gameObject.transform;
            Destroy(collision.gameObject);
            Vector3 explore = temp.position;
            Instantiate(bigBroken, explore, temp.rotation);
        }
        else if (collision.gameObject.tag == "little" && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("enterOnCollisionEnter2D");
            GameManager.addScore(1);
            Transform temp = collision.gameObject.transform;
            Destroy(collision.gameObject);
            Vector3 explore = temp.position;
            Instantiate(smallBroken, explore, temp.rotation);
        }
    }
    void PureForTest()
    {
        if (Input.GetKey(KeyCode.M))
        {
            GameManager.addScore(5);
        }
    }
}
