using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Attacking2State : DinoBaseState
{
    private Dino _Dino;
    private float time;
    private float backTime;
    public bool canMove;
    private float start;
    public Attacking2State(Dino Dino)
    {
        _Dino = Dino;
        time = 0.3f;
        backTime = 0.1f;
        canMove = false;
        //Debug.Log("------------------------Dino in Attacking1State~!（进入攻击二号状态！）");
        Update();
        _Dino.StartCoroutine(OnWaitMethod());
    }

    public void Update()//实现第一击
    {
        _Dino.animator.SetBool("AttackKick", true);
        _Dino.animator.SetBool("AttackBox", false);
    }

    public void HandleInput()
    {
        if (canMove)
        {
            if (Input.GetKey(KeyCode.LeftControl))//第三连击
            {
                _Dino.SetDinoState(new Attacking3State(_Dino));
            }
            else if (Time.time - start > backTime)
            {
                _Dino.animator.SetBool("AttackKick", false);
                _Dino.SetDinoState(new StandingState(_Dino));
            }
        }
    }
    IEnumerator OnWaitMethod()
    {
        yield return new WaitForSeconds(time);
        canMove = true;
        start = Time.time;
    }
}
