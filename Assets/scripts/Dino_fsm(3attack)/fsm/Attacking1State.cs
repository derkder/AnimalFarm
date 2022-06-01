using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Attacking1State : DinoBaseState
{
    private Dino _Dino;
    private float time;
    private float backTime;
    private bool canMove;
    private float start;
    public Attacking1State(Dino Dino)//第一击挥拳
    {
        _Dino = Dino;
        time = 0.3f; //硬直时间
        backTime = 0.2f;//按下后多久按下就算数
        canMove = false;
        //Debug.Log("------------------------Dino in Attacking1State~!（进入攻击一号状态！）");
        Update();
        _Dino.StartCoroutine(OnWaitMethod());
        //因为这里没有继承monobehavior类，所以协程方法得从别的地方拿
    }

    public void Update()//实现第一击
    {
        _Dino.animator.SetBool("AttackBox", true);
    }

    public void HandleInput()//本来进了这个状态每帧都要调用
    {
        if (canMove)//硬直过后
        {
            //Debug.Log("硬直过后");
            if (Input.GetKey(KeyCode.LeftControl))//第二连击,现在是按着ctrl不松手就行，不然貌似会卡，后面再改
            {
                _Dino.SetDinoState(new Attacking2State(_Dino));
            }
            else if (Time.time - start > backTime)
            {
                _Dino.animator.SetBool("AttackBox", false);
                _Dino.SetDinoState(new StandingState(_Dino));
                //回到原始状态，但其实动画控制是用existime回到idle的
            }
        }
    }
    IEnumerator OnWaitMethod()
    {
        yield return new WaitForSeconds(time);
        canMove=true;//再经过time
        start = Time.time;
    }
    //但是只要在给定时间内按出来就好
    //按出第一个attack之后的3s有一帧按了攻击就行
}