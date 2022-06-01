using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
public class StandingState : DinoBaseState
{
    private Dino _Dino;
    public StandingState(Dino Dino)//只是区别于攻击和技能状态的初始状态类罢了，不是真的idle
    {
        _Dino = Dino;
        //Debug.Log("------------------------Dino in StandingState~!（进入站立状态！）");
        Update();
    }

    public void Update()
    {
        _Dino.animator.SetBool("AttackTail", false);
    }

    public void HandleInput()
    {
        //Debug.Log("站立状态handleInput");
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //Debug.Log("get KeyCode.LeftControl!");
            _Dino.SetDinoState(new Attacking1State(_Dino));
        }
    }
}
