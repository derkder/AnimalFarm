using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MovingState : DinoBaseState//运行有问题，会一直往下坠，决定fsm里不搞这个状态了
{
    private Dino _Dino;
    private Vector3 moveDirection = Vector3.zero;
    private float inputX, inputY;
    public float stopX, stopY;
    public bool isMove = false;
    private float preinputX = 1;


    public MovingState(Dino Dino)//相当于start了大概,，唯一改原方法成功的地方！
    {
        _Dino = Dino;
        Debug.Log("------------------------Dino in MovingState~!（进入移动状态！）");
        Update();
    }

    public void Update()//执行逻辑写这里
    {
        inputX = Input.GetAxisRaw("Horizontal");//只返回-1，0，1
        inputY = Input.GetAxisRaw("Vertical");
        Vector2 input = (Vector3.right * inputX + _Dino.transform.up * inputY).normalized;
        //_Dino.rigidbody.velocity = input * _Dino.speed;
        //rigidbody.velocity = new Vector2(inputX * speed, inputY * speed);
        if (input != Vector2.zero)
        {
            _Dino.animator.SetBool("isMoving", true);
            stopX = inputX;
            stopY = inputY;
            if (inputX != 0 && inputX != preinputX)
            {
                _Dino.transform.Rotate(0f, 180f, 0f);
                preinputX = inputX;
            }
        }
        else
        {
            _Dino.animator.SetBool("isMoving", false);
        }
        _Dino.animator.SetFloat("InputX", stopX);
        _Dino.animator.SetFloat("InputY", stopY);
    }

    public void HandleInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))//值按下这个瞬间得动作才算
        {
            Update();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("get KeyCode.LeftControl!");
            _Dino.SetDinoState(new Attacking1State(_Dino));
        }
        else
        {
            _Dino.SetDinoState(new StandingState(_Dino));
        }
    }
}
