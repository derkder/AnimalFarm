using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    public enum bossStates { FOLLOW, ATTACK, HIT };//��������ȫ����attack��
    public bool inHitState;
    //Ĭ��״̬һֱ׷��player��
    public bool isStage2;
    public bool test;
    public bool transToHit;
    public bool pretransToHit;
    public bossStates bossState;
    public Animator anim;
    public Transform attackTarget;
    private float lastAttackTime;
    public Rigidbody2D rb;
    public Collider2D coll;
    private Vector3 movement;//�����ƶ��ķ���
    public int skillStage = 0;
    [Header("�����������")]
    public bool inSkill=false;
    public bool isMove;//patrol״̬
    public bool isEarth;
    public bool isMud;
    public bool isTail;
    public bool isHit;
    public bool isHurt;
    public bool isBump;
    public bool isEarth2;
    public bool isDead;
    [Header("Patrol State")]
    public float sightRadius;//�����������Զ�ṥ��
    public float attackRange;//�������ᴥ�����򶯻�
    public Vector3 startPos;
    [Header("����������")]
    public int hp = 10;
    public float speed;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    void Start()
    {
        isMove = true;
        startPos = transform.position;
        PlayerMovement.Instance.OnAttack -= GetHitByPlayer;
        PlayerMovement.Instance.OnAttack += GetHitByPlayer;
        PlayerMovement.Instance.OnSkill -= GetHitByPlayer;
        PlayerMovement.Instance.OnSkill += GetHitByPlayer;
    }

    void Update()
    {
        SwitchAnimation();
        if (!isStage2)
        {
            SwitchStates();
            if (isStage2)
            {
                bossState = bossStates.FOLLOW;
            }
        }
        else
        {
            skillStage = 0;
            SwitchStates2();
        }
    }
    void SwitchAnimation()
    {
        anim.SetBool("isMove", isMove);
        anim.SetBool("isEarth", isEarth);
        anim.SetBool("isMud", isMud);
        anim.SetBool("isTail", isTail);
        anim.SetBool("isHit", isHit);
        anim.SetBool("isHurt", isHurt);
        anim.SetBool("isBump", isBump);
        anim.SetBool("isEarth2", isEarth2);
        anim.SetBool("isDead", isDead);
        anim.SetBool("isStage2", isStage2);
    }
    void SwitchStates()
    {
        if (transToHit)
        {
            bossState = bossStates.HIT;

        }
        switch (bossState)
        {
            case bossStates.FOLLOW://׷���Ĺ���
                if (TargetInAttackRange())
                {
                    bossState = bossStates.ATTACK;
                }
                else//׷��
                {
                    ChaseTarget(attackTarget.position);
                }
                break;
            case bossStates.ATTACK:
                Debug.Log("ATTACK");
                if (attackTarget.position.x > transform.position.x && transform.rotation.y == 0)
                {
                    transform.Rotate(0f, 180f, 0f);
                }
                if (skillStage == 0&&!inSkill)
                {
                    Debug.Log("attack skillStage == 0");
                    skillStage++;
                    isEarth = true;
                    inSkill = true;
                    StartCoroutine(OnWaitAttackMethod());
                }
                else if(skillStage == 1)
                {
                    skillStage++;
                    isMud = true;
                    inSkill = true;
                    StartCoroutine(OnWaitAttackMethod());
                }
                else if (skillStage == 2)
                {
                    skillStage=0;
                    isTail = true;
                    inSkill = true;
                    StartCoroutine(OnWaitAttackMethod());
                }
                ChaseTarget(attackTarget.position);
                if (!TargetInAttackRange())
                {
                    bossState = bossStates.FOLLOW;
                }
                break;
            case bossStates.HIT:
                isHit = true;
                if (!inHitState)
                {
                    hp--;
                    StartCoroutine(OnWaitHitMethod());
                }
                if (hp <= 5)
                {
                    isHurt = true;
                    hp--;
                    StartCoroutine(OnWaitHurtMethod());
                    isStage2 = true;
                }
                bossState = bossStates.ATTACK;
                break;
        }
    }
    void SwitchStates2()
    {
        Debug.Log("SwitchStates2");
        //bossState = bossStates.FOLLOW;
        if (transToHit)
        {
            bossState = bossStates.HIT;

        }
        switch (bossState)
        {
            case bossStates.FOLLOW://׷���Ĺ���
                if (TargetInAttackRange())
                {
                    bossState = bossStates.ATTACK;
                }
                else//׷��
                {
                    ChaseTarget(attackTarget.position);
                }
                break;
            case bossStates.ATTACK:
                if (attackTarget.position.x > transform.position.x && transform.rotation.y == 0)
                {
                    transform.Rotate(0f, 180f, 0f);
                }
                if (inSkill) Debug.Log("inSkill_true");
                if (skillStage == 0)
                {
                    Debug.Log("inskillStage == 0");
                    skillStage++;
                    isEarth2 = true;
                    anim.SetBool("isEarth2", isEarth2);
                    StartCoroutine(OnWaitAttackMethod());
                }
                else if (skillStage == 1)
                {
                    skillStage=0;
                    isBump = true;
                    StartCoroutine(OnWaitAttackMethod());
                }
                else
                {
                    skillStage = 0;
                }
                ChaseTarget(attackTarget.position);
                if (!TargetInAttackRange())
                {
                    bossState = bossStates.FOLLOW;
                }
                break;
            case bossStates.HIT:
                isHit = true;
                if (!inHitState)
                {
                    hp--;
                    StartCoroutine(OnWaitHitMethod());
                }
                if (hp == 0)
                {
                    isDead = true;
                    StartCoroutine(OnWaitDeadMethod());
                }
                bossState = bossStates.ATTACK;
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
    bool FoundPlayer()//����
    {
        if (Vector3.Distance(attackTarget.position, transform.position) < 5)
        {
            return true;
        }
        return false;
    }
    bool TargetInAttackRange()
    {
        if (Vector3.Distance(attackTarget.position, transform.position) < attackRange)
        {
            return true;
        }
        return false;
    }
    public void GetHitByPlayer(GameObject player)//���ﴦ�ڹ���״̬�ͻ�������
    {
        //��ʵ����������ô��Σ����溯����ı�������player��Ҳ����attackTarget
        if (TargetInAttackRange())//������Ĺ�������˵������������
        {
            transToHit = true;
            if (!pretransToHit && transToHit)
            {
                //hp--;
            }
            pretransToHit = transToHit;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,attackRange);
    }
    //����д��Э�̾�Ϊ��ʱ��,��Ϊsipne�����޷���animation preview
    #region
    IEnumerator OnWaitHitMethod()
    {
        inHitState = true;
        yield return new WaitForSeconds(0.5f);
        isHit = false;
        transToHit = false;
        pretransToHit = transToHit;
        inHitState = false;
    }
    IEnumerator OnWaitAttackMethod()
    {
        inSkill = true;
        yield return new WaitForSeconds(0.6f);
        inSkill = false;
        isEarth = false;
        //isEarth2 = false;
        isMud = false;
        isTail = false;
        isBump = false;
    }
    IEnumerator OnWaitDeadMethod()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Destroy(gameObject);
    }
    IEnumerator OnWaitHurtMethod()
    {
        yield return new WaitForSeconds(0.4f);
        if(isHurt) isHurt = false;
    }
    #endregion
    private void OnColliderEnter2D(Collider2D other)//
    {
        if (other.CompareTag("Player") && inSkill)
        {
            other.GetComponent<PlayerMovement>().Injured();
        }
    }
}
