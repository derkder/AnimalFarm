using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour//������animals����
{
    public bool isInCoroutine;
    public enum sheepStates { Patrol, FOLLOW, ATTACK, HIT };
    public bool transToHit;//��������ý���hit״̬�ˣ���gethit�����ﱻ��ֵ
    public sheepStates sheepState;
    public Animator anim;
    public float speed;
    public Transform followTarget;
    public Transform attackTarget;
    private float lastAttackTime;
    public Rigidbody2D rb;
    public Collider2D coll;
    public Vector3 movement;//�����ƶ��ķ���

    //[HideInInspector]
    [Header("�����������")]
    public bool isMove;//patrol״̬
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    public bool isSkill;
    [Header("Patrol State")]
    public float patrolPos=4;//Ѳ�����ƶ���λ��
    public float patrolRange=15;//��ҵ����Զ�ἤ�����
    public float sightRadius=15;//ũ�򵽴��Զ�ṥ��
    public float attackRange=6;//�������ᴥ�����򶯻�
    public Vector3 startPos;

    [Header("��������")]
    public int hp=3 ;
    public int attacktimes=2;//���뼼���ͷ�
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
            case sheepStates.Patrol://��ũ��һ��������Ѳ�ߣ����ǳ���״̬�Ͳ����ٽ�����
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
                if (FoundPlayer())//�л������棬�ҵ����
                {
                    sheepState = sheepStates.FOLLOW;
                }
                break;
            case sheepStates.FOLLOW://����Ĺ���
                ChaseTarget(followTarget.transform);
                if (TargetInAttackRange())//�ҵ�����,����ֵatttackTarget
                {
                    sheepState = sheepStates.ATTACK;
                }
                break;
            case sheepStates.ATTACK:
                if (TargetInAttackRange()&&attackTarget)//׷�ŵ���
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
                    if (Vector3.Distance(attackTarget.transform.position,transform.position)<attackRange)//�������Թ�����
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
                            if (!isInCoroutine) StartCoroutine(OnWaitAttackMethod());//Ӧ��ȷ����һ��û��coroutine�˲�start��������úö�
                        }
                    }
                }
                else//attacktargete��ʧ
                {
                    sheepState = sheepStates.FOLLOW;
                }
                break;
            case sheepStates.HIT://ֻҪ���Ա���transToHitΪ�����������
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
    }//�ص�ԭ����λ��
    bool FoundPlayer()
    {
        if (Vector3.Distance(followTarget.position, transform.position) < patrolRange)
        {
            return true;
        }
        return false;
    }
    bool TargetInAttackRange()//����attacktarget
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
    public void GetHit(Vector3 attack)//������˵����ũ��||boss����
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
    //��ײ��⣬������ҵĹ����ü����¼��жϣ�����������ײ
    //��Ϊ��ҵĹ������˶�̬fsm��������дһ���ҿ϶�����ô����
    private void OnColliderEnter2D(Collider2D other)
    {
        if (other.CompareTag("farmer") && (sheepState == sheepStates.ATTACK))
        {
            other.GetComponent<farmer>().GetHit(Vector3.right);
        }
    }

}
