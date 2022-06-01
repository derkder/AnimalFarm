using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Attacking3State : DinoBaseState
{
    private Dino _Dino;
    private float time=0.3f;
    public Attacking3State(Dino Dino)
    {
        _Dino = Dino;
        //Debug.Log("------------------------Dino in Attacking1State~!（进入攻击三号状态！）");
        //_Dino.transform.Rotate(0f, 180f, 0f);
        Update();
        _Dino.StartCoroutine(OnWaitMethod());
        _Dino.animator.SetBool("AttackKick", false);
    }

    public void Update()//实现第三击
    {
        _Dino.animator.SetBool("AttackTail", true);
    }

    public void HandleInput()
    {
        //_Dino.transform.Rotate(0f, 180f, 0f);
        _Dino.SetDinoState(new StandingState(_Dino));
    }
    IEnumerator OnWaitMethod()
    {
        yield return new WaitForSeconds(time);
    }
}
