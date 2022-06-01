using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseAnimal : MonoBehaviour//���н�ɫò��ֻ�м�����ز�һ����
{
    //�����Ա������ʱ��д��public��������ʱ��ĳ�getsetģʽ
    //��������д�����߼����˸о�û���⵫��inspectorҪ��ը��
    //������ʱ����json��ûʱ���ֱ��д��Ա��������
    public enum AnimalStates{ LOCK, FOLLOW, ATTACK,HIT };//IDLE�������������һ�ж���
    //����ȵĶ�������IDLE����ܷ���attack״̬�DEATH����HIT��
    public AnimalStates animalState;
    public NavMeshAgent agent;
    public Animator anim;
    public float speed;
    public GameObject followTarget;
    public GameObject attackTarget;
    private float lastAttackTime;
    public Collider coll;
    [Header("�����������")]
    public bool isLock;
    public bool isFollow;
    public bool isAttack;
    public bool isHit;
    //�����״̬��ֻ�ж����ı��߼���С״̬
    public bool isIdle;
    public bool isWalk;
    public bool isFree;
    public bool isDead;
    private Quaternion guardRotation;
    public Vector3 wayPoint1;
    public Vector3 wayPoint2;
    [Header("Patrol State")]
    public float patrolRange;//��ҵ����Զ�ἤ�����
    public float sightRadius;//ũ�򵽴��Զ�ṥ��
    [Header("��������")]
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
            case AnimalStates.LOCK://һ�����˾ͽ�������״̬
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
                    anim.SetBool("Free", isFree);//���Ž�ȵĶ���
                    animalState = AnimalStates.FOLLOW;
                }
                break;
            case AnimalStates.FOLLOW:
                isWalk = false;
                isFollow = true;
                agent.speed = speed;//����ĳɻ�õ�����ٶ�
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
            case AnimalStates.ATTACK://��������ҵ����˵������Զ�룬�����˭��
                int attackTimes = 0;
                if (attackTimes < 2)
                {
                    //��ͨ����
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
    public virtual void SetSkill()//ò����ΨһҪ��д���ࣿ
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


