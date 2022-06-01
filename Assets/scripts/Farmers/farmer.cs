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
    private Vector3 movement;//�����ƶ��ķ���
    [Header("�����������")]
    public bool isMove;//patrol״̬
    public bool isAttack;
    public bool isHit;
    public bool isDead;
    private Quaternion guardRotation;
    [Header("Patrol State")]
    public float patrolRange;//��ҵ����Զ�ἤ��׷��
    public float sightRadius;//ũ�򵽴��Զ�ṥ��
    public float attackRange;//�������ᴥ�����򶯻�
    public Vector3 startPos;
    [Header("ũ������")]
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
                if (FoundPlayer())//�л���׷��
                {
                    farmerState = farmerStates.FOLLOW;
                }
                break;
            case farmerStates.FOLLOW://׷���Ĺ���
                if (TargetInAttackRange())
                {
                    farmerState = farmerStates.ATTACK;
                }
                else if (FoundPlayer())//׷��
                {
                    ChaseTarget(attackTarget.position);
                }
                else//����Զ���ص�patrol״̬
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
    public void GetHit(Vector3 attack)//������˵����animal����
    {
        transToHit = true;
        if (!pretransToHit && transToHit)
        {
            hp--;
        }
        pretransToHit = transToHit;
    }
    public void GetHitByPlayer(GameObject player)//���ﴦ�ڹ���״̬�ͻ�������
    {
        //��ʵ����������ô��Σ����溯����ı�������player��Ҳ����attackTarget
        if (TargetInAttackRange())//������Ĺ�������˵������������
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
    //����д��Э�̾�Ϊ��ʱ��,��Ϊsipne�����޷���animation preview
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
