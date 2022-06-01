using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Dino : MonoBehaviour
{
    DinoBaseState _state;
    //public event Action OnAttackClicked;//也许把event写进动物里会更好？
    public Animator animator;
    public new Rigidbody2D rigidbody;
    public Collider2D coll;
    public float AttackWaitTime = 0.8F;//三连击过程中，每次攻击的硬直时间
    public float AttacRetTime = 1.0F;//三连击过程中，每次攻击按下后多久按下就算数
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
