using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseAnimal : MonoBehaviour//所有角色貌似只有技能相关不一样？
{
    //基类成员变量暂时先写成public，后面有时间改成getset模式
    //现在这样写程序逻辑个人感觉没问题但是inspector要爆炸了
    //技能有时间打成json表，没时间就直接写成员变量里了
    public enum AnimalStates{ LOCK, FOLLOW, ATTACK,HIT };//IDLE包括在笼子里的一切动画
    //被解救的动画放在IDLE里，技能放在attack状态里，DEATH放在HIT里
    public AnimalStates animalState;
    public NavMeshAgent agent;
    public Animator anim;
    public float speed;
    public GameObject followTarget;
    public GameObject attackTarget;
    private float lastAttackTime;
    public Collider coll;
    [Header("动画属性相关")]
    public bool isLock;
    public bool isFollow;
    public bool isAttack;
    public bool isHit;
    //上面大状态里只有动画改变逻辑的小状态
    public bool isIdle;
    public bool isWalk;
    public bool isFree;
    public bool isDead;
    private Quaternion guardRotation;
    public Vector3 wayPoint1;
    public Vector3 wayPoint2;
    [Header("Patrol State")]
    public float patrolRange;//玩家到达多远会激活跟随
    public float sightRadius;//农夫到达多远会攻击
    [Header("动物属性")]
    public int hp;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        //  characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        guardRotation = transform.rotation;
    }
    void Start()
    {
        isIdle = true;
        Debug.Log("BaseAnimalStart");
    }

    void Update()
    {
        SwitchAnimation();
        if (isDead)
        {
            Destroy(gameObject);
        }
        SwitchStates();
    }
    void SwitchAnimation()
    {
        anim.SetBool("Idle", isIdle);
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Free", isFree);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Attack", isAttack);
        anim.SetBool("Hit", isHit);
        anim.SetBool("Dead", isDead);
    }
    void SwitchStates()
    {
        switch (animalState)
        {
            case AnimalStates.LOCK://一个出了就进不来的状态
                isWalk = true;
                agent.speed = speed * 0.5f;
                Vector3 wayPoint=wayPoint1;
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    agent.destination = wayPoint;
                }
                else
                {
                    if (wayPoint == wayPoint1) wayPoint = wayPoint2;
                    else wayPoint = wayPoint1;
                }
                if (FoundPlayer())
                {
                    anim.SetBool("Free", isFree);//播放解救的动画
                    animalState = AnimalStates.FOLLOW;
                }
                break;
            case AnimalStates.FOLLOW:
                isWalk = false;
                isFollow = true;
                agent.speed = speed;//后面改成获得的玩家速度
                if(FoundEnemy())
                {
                    animalState = AnimalStates.ATTACK;
                }
                else if (Vector3.Distance(followTarget.transform.position, transform.position) >= 5)
                {
                    transform.LookAt(followTarget.transform);
                    agent.destination = followTarget.transform.position;
                }
                break;
            case AnimalStates.ATTACK://所以如果找到敌人但玩家又远离，动物跟谁呢
                int attackTimes = 0;
                if (attackTimes < 2)
                {
                    //普通攻击
                    attackTimes++;
                }
                else
                {
                    SetSkill();
                    attackTimes = 0;
                }
                if (EnemyDead())
                {
                    animalState = AnimalStates.FOLLOW;
                }
                break;
            case AnimalStates.HIT:
                hp--;
                if (hp == 0)
                {
                    isDead = true;
                }
                else
                {
                    isHit = true;
                }
                break;
        }
    }
    void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }
    void MoveToAttackTarget(GameObject target)
    {

    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, patrolRange);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                isFollow = true;
                followTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool FoundEnemy()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                isFollow = false;
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool EnemyDead()
    {
        if(attackTarget == null)
        {
            return true;
        }
        return false;
    }
    public virtual void SetSkill()//貌似是唯一要重写的类？
    {

    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= 5;
        else
            return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= 5;
        else
            return false;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}


