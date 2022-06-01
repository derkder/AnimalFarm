using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MovingState : DinoBaseState//���������⣬��һֱ����׹������fsm�ﲻ�����״̬��
{
    private Dino _Dino;
    private Vector3 moveDirection = Vector3.zero;
    private float inputX, inputY;
    public float stopX, stopY;
    public bool isMove = false;
    private float preinputX = 1;


    public MovingState(Dino Dino)//�൱��start�˴��,��Ψһ��ԭ�����ɹ��ĵط���
    {
        _Dino = Dino;
        Debug.Log("------------------------Dino in MovingState~!�������ƶ�״̬����");
        Update();
    }

    public void Update()//ִ���߼�д����
    {
        inputX = Input.GetAxisRaw("Horizontal");//ֻ����-1��0��1
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
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))//ֵ�������˲��ö�������
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
