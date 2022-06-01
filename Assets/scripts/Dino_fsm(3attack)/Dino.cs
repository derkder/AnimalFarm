using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Dino : MonoBehaviour
{
    DinoBaseState _state;
    //public event Action OnAttackClicked;//Ҳ���eventд�����������ã�
    public Animator animator;
    public new Rigidbody2D rigidbody;
    public Collider2D coll;
    public float AttackWaitTime = 0.8F;//�����������У�ÿ�ι�����Ӳֱʱ��
    public float AttacRetTime = 1.0F;//�����������У�ÿ�ι������º��ð��¾�����
    public bool canChangeState;
    public void SetDinoState(DinoBaseState newState)
    {
        _state = newState;
    }


    void Start()
    {
        //controll = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        _state = new StandingState(this);
    }

    public void FixedUpdate()
    {
        //rigidbody.velocity = rigidbody.velocity;
        _state.HandleInput();
    }

}
