using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour//给所有animals用了
{
    public bool isInCoroutine;
    public enum sheepStates { Patrol, FOLLOW, ATTACK, HIT };
    public bool transToHit;//用来传达该进入hit状态了，在gethit函数里被赋值
    public sheepStates sheepState;
    public Animator anim;
    public float speed;
    public Transform followTarget;
    public Transform attackTarget;
    private float lastAttackTime;
    public Rigidbody2D rb;
    public Collider2D coll;
    public Vector3 movement;//现在移动的方向

    //[HideInInspector]
    [Header("动画属性相关")]
    public bool isMove;//patrol状态
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    public bool isSkill;
    [Header("Patrol State")]
    public float patrolPos=4;//巡逻是移动的位移
    public float patrolRange=15;//玩家到达多远会激活跟随
    public float sightRadius=15;//农夫到达多远会攻击
    public float attackRange=6;//到达多近会触发击打动画
    public Vector3 startPos;

    [Header("动物属性")]
    public int hp=3 ;
    public int attacktimes=2;//距离技能释放
    private void Awake()
    {
        anim = GetComponent<Animator>();
        //characterStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    void Start()
    {
        isMove = true;
        startPos = transform.position;
        movement.y = 0;
        movement.x = speed;
    }

    void Update()
    {
        SwitchAnimation();
        SwitchStates();

    }
    void SwitchAnimation()
    {
        anim.SetBool("isMove", isMove);
        anim.SetBool("isAttack", isAttack);
        anim.SetBool("isSkill", isSkill);
        anim.SetBool("isHit", isHit);
        anim.SetBool("isDead", isDead);
    }
    void SwitchStates()
    {
        if (transToHit)
        {
            sheepState = sheepStates.HIT;
        }
        switch (sheepState)
        {
            case sheepStates.Patrol://和农夫一样，就是巡逻，但是出了状态就不会再进来了
                isMove = true;
                transform.position += movement * Time.deltaTime;
                if (transform.position.x <= startPos.x - patrolPos)
                {
                    //transform.Rotate(0f, 180f, 0f);
                    movement.x = speed;
                }
                else if (transform.position.x >= startPos.x + patrolPos)
                {
                    //transform.Rotate(0f, 180f, 0f);
                    movement.x = -speed;
                }
                if (FoundPlayer())//切换到跟随，找到玩家
                {
                    sheepState = sheepStates.FOLLOW;
                }
                break;
            case sheepStates.FOLLOW://跟随的过程
                ChaseTarget(followTarget.transform);
                if (TargetInAttackRange())//找到敌人,并赋值atttackTarget
                {
                    sheepState = sheepStates.ATTACK;
                }
                break;
            case sheepStates.ATTACK:
                if (TargetInAttackRange()&&attackTarget)//追着敌人
                {
                    Debug.Log("TargetInAttackRange()&&attackTarget");
                    if (attackTarget.position.x > transform.position.x && transform.rotation.y == 0)
                    {
                        transform.Rotate(0f, 180f, 0f);
                    }
                    else if(attackTarget.position.x < transform.position.x && transform.rotation.y == 180)
                    {
                        transform.Rotate(0f, 180f, 0f);
                    }
                    ChaseTarget(attackTarget.transform);
                    if (Vector3.Distance(attackTarget.transform.position,transform.position)<attackRange)//近到可以攻击了
                    {
                        if (attacktimes < 0)
                        {
                            isSkill = true;
                            StartCoroutine(OnWaitSkillMethod());
                            attacktimes = 2;
                        }
                        else
                        {
                            isAttack = true;
                            if (!isInCoroutine) StartCoroutine(OnWaitAttackMethod());//应该确保上一个没有coroutine了踩start，否则调用好多
                        }
                    }
                }
                else//attacktargete消失
                {
                    sheepState = sheepStates.FOLLOW;
                }
                break;
            case sheepStates.HIT://只要属性变量transToHit为真就跳到这里
                isHit = true;
                StartCoroutine(OnWaitHitMethod());
                if (hp == 0)
                {
                    isDead = true;
                    StartCoroutine(OnWaitDeadMethod());
                }
                //sheepState = sheepStates.ATTACK;
                break;
        }
    }

    void ChaseTarget(Transform target)
    {
        transform.position += movement * Time.deltaTime;
        if (transform.position.x >= target.position.x)
        {
            movement.x = -speed;
        }
        else if (transform.position.x <= target.position.x)
        {
            movement.x = speed;
        }
        if (transform.position.y >= target.position.y)
        {
            movement.y = -speed;
        }
        else if (transform.position.y <= target.position.y)
        {
            movement.y = speed;
        }
    }
    void backToOri()
    {
        while (Vector3.Distance(startPos, transform.position) > 0.1f)
        {
            if (transform.position.x >= startPos.x)
            {
                movement.x = -speed;
            }
            else if (transform.position.x <= startPos.x)
            {
                movement.x = speed;
            }
            if (transform.position.y >= startPos.y)
            {
                movement.y = -speed;
            }
            else if (transform.position.y <= startPos.y)
            {
                movement.y = speed;
            }
        }
    }//回到原来的位置
    bool FoundPlayer()
    {
        if (Vector3.Distance(followTarget.position, transform.position) < patrolRange)
        {
            return true;
        }
        return false;
    }
    bool TargetInAttackRange()//设置attacktarget
    {
        GameObject[] farmers;
        farmers = GameObject.FindGameObjectsWithTag("farmer");
        if (farmers != null)
        {
            Debug.Log("farmers != null");
        }
        for(int i = 0; i < farmers.Length; i++)
        {
            if (Vector3.Distance(farmers[i].transform.position, transform.position)<sightRadius)
            {
                attackTarget = farmers[i].transform;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    public void GetHit(Vector3 attack)//被调用说明被农夫||boss打到了
    {
        transToHit = true;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
    IEnumerator OnWaitHitMethod()
    {
        isInCoroutine = true;
        yield return new WaitForSeconds(0.5f);
        isHit = false;
        transToHit = false;
        sheepState = sheepStates.ATTACK;
        isInCoroutine = false;
    }
    IEnumerator OnWaitAttackMethod()
    {
        isInCoroutine = true;
        Debug.Log("OnWaitAttackMethod()before");
        yield return new WaitForSecondsRealtime(0.5f);
        isAttack = false;
        attacktimes--;
        Debug.Log("OnWaitAttackMethod()after");
        isInCoroutine = false;
;    }
    IEnumerator OnWaitDeadMethod()
    {
        isInCoroutine = true;
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
        isInCoroutine = false;
    }
    IEnumerator OnWaitSkillMethod()
    {
        isInCoroutine = true;
        yield return new WaitForSeconds(0.8f);
        isSkill = false;
        isInCoroutine = false;
    }
    //碰撞检测，除了玩家的攻击用监听事件判断，其他都用碰撞
    //因为玩家的攻击用了多态fsm，再让我写一次我肯定不这么绕了
    private void OnColliderEnter2D(Collider2D other)
    {
        if (other.CompareTag("farmer") && (sheepState == sheepStates.ATTACK))
        {
            other.GetComponent<farmer>().GetHit(Vector3.right);
        }
    }

}
