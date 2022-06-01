using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class farmer : MonoBehaviour
{
    public enum farmerStates {Patrol , FOLLOW, ATTACK, HIT };
    public bool test;
    public bool transToHit;
    public bool pretransToHit;
    public farmerStates farmerState;
    public Animator anim;
    public float speed;
    public Transform attackTarget;
    private float lastAttackTime;
    public Rigidbody2D rb;
    public Collider2D  coll;
    private Vector3 movement;//现在移动的方向
    [Header("动画属性相关")]
    public bool isMove;//patrol状态
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    private Quaternion guardRotation;
    [Header("Patrol State")]
    public float patrolRange;//玩家到达多远会激活追击
    public float sightRadius;//农夫到达多远会攻击
    public float attackRange;//到达多近会触发击打动画
    public Vector3 startPos;
    [Header("农夫属性")]
    public int hp=3;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        //characterStats = GetComponent<CharacterStats>();
        rb= GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        guardRotation = transform.rotation;
    }
    void Start()
    {
        isMove = true;
        startPos = transform.position;
        movement.y = speed;
        movement.x = 0;
        PlayerMovement.Instance.OnAttack -= GetHitByPlayer;
        PlayerMovement.Instance.OnAttack += GetHitByPlayer;
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
        anim.SetBool("isHit", isHit);
        anim.SetBool("isDead", isDead);
    }
    void SwitchStates()
    {
        if (transToHit)
        {
            farmerState = farmerStates.HIT;
        }
        switch (farmerState)
        {
            case farmerStates.Patrol:
                isMove = true;
                transform.position += movement * Time.deltaTime;
                if ((transform.position.x-startPos.x)>1f)
                {
                    ChaseTarget(startPos);
                    Debug.Log("backtoguard");
                }
                else
                {
                    movement.x = 0;
                    if (transform.position.y <= startPos.y - 10)
                    {
                        movement.y = speed;
                        test = true;
                    }
                    else if (transform.position.y >= startPos.y + 10)
                    {
                        movement.y = -speed;
                        test = true;
                    }
                }
                if (FoundPlayer())//切换到追击
                {
                    farmerState = farmerStates.FOLLOW;
                }
                break;
            case farmerStates.FOLLOW://追击的过程
                if (TargetInAttackRange())
                {
                    farmerState = farmerStates.ATTACK;
                }
                else if (FoundPlayer())//追击
                {
                    ChaseTarget(attackTarget.position);
                }
                else//隔得远，回到patrol状态
                {
                    farmerState = farmerStates.Patrol;
                }
                break;
            case farmerStates.ATTACK:
                if (attackTarget.position.x > transform.position.x&&transform.rotation.y==0)
                {
                    transform.Rotate(0f, 180f, 0f);
                }
                isAttack = true;
                StartCoroutine(OnWaitAttackMethod());
                ChaseTarget(attackTarget.position);
                if (!TargetInAttackRange())
                {
                    farmerState = farmerStates.FOLLOW;
                }
                else if (!FoundPlayer())
                {
                    farmerState = farmerStates.Patrol;
                }
                break;
            case farmerStates.HIT:
                isHit = true;
                StartCoroutine(OnWaitHitMethod());
                if (hp == 0)
                {
                    isDead = true;
                    StartCoroutine(OnWaitDeadMethod());
                }
                farmerState = farmerStates.ATTACK;
                break;
        }
    }
    void ChaseTarget(Vector3 target)
    {
        transform.position += movement * Time.deltaTime;
        if (transform.position.x >= target.x)
        {
            movement.x = -speed;
        }
        else if (transform.position.x <= target.x)
        {
            movement.x = speed;
        }
        if (transform.position.y >= target.y)
        {
            movement.y = -speed;
        }
        else if (transform.position.y <= target.y)
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
    }
    bool FoundPlayer()
    {
        if (Vector3.Distance(attackTarget.position,transform.position)< patrolRange)
        {
            return true;
        }
        return false;
    }
    bool TargetInAttackRange()
    {
        if (Vector3.Distance(attackTarget.position, transform.position) < sightRadius)
        {
            return true;
        }
        return false;
    }
    public void GetHit(Vector3 attack)//被调用说明被animal打到了
    {
        transToHit = true;
        if (!pretransToHit && transToHit)
        {
            hp--;
        }
        pretransToHit = transToHit;
    }
    public void GetHitByPlayer(GameObject player)//人物处于攻击状态就会调用这个
    {
        //其实这个函数不用传参，下面函数里的变量就是player，也就是attackTarget
        if (TargetInAttackRange())//如果靠的够近，就说明被攻击到了
        {
            transToHit = true;
            if (!pretransToHit && transToHit)
            {
                hp--;
            }
            pretransToHit = transToHit;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRange);
    }
    //这里写的协程均为计时器,因为sipne动画无法用animation preview
    IEnumerator OnWaitHitMethod()
    {
        yield return new WaitForSeconds(0.5f);
        isHit = false;
        transToHit = false;
        pretransToHit = transToHit;
    }
    IEnumerator OnWaitAttackMethod()
    {
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
    }
    IEnumerator OnWaitDeadMethod()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.addScore(6);
        Destroy(gameObject);
    }

    private void OnColliderEnter2D(Collider2D other)
    {
        if (other.CompareTag("animals")&& (farmerState == farmerStates.ATTACK))
        {
            other.GetComponent<Sheep>().GetHit(Vector3.right);
        }
    }
}
