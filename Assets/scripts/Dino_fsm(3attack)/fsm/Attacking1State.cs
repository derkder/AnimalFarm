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
    public Attacking1State(Dino Dino)//��һ����ȭ
    {
        _Dino = Dino;
        time = 0.3f; //Ӳֱʱ��
        backTime = 0.2f;//���º��ð��¾�����
        canMove = false;
        //Debug.Log("------------------------Dino in Attacking1State~!�����빥��һ��״̬����");
        Update();
        _Dino.StartCoroutine(OnWaitMethod());
        //��Ϊ����û�м̳�monobehavior�࣬����Э�̷����ôӱ�ĵط���
    }

    public void Update()//ʵ�ֵ�һ��
    {
        _Dino.animator.SetBool("AttackBox", true);
    }

    public void HandleInput()//�����������״̬ÿ֡��Ҫ����
    {
        if (canMove)//Ӳֱ����
        {
            //Debug.Log("Ӳֱ����");
            if (Input.GetKey(KeyCode.LeftControl))//�ڶ�����,�����ǰ���ctrl�����־��У���Ȼò�ƻῨ�������ٸ�
            {
                _Dino.SetDinoState(new Attacking2State(_Dino));
            }
            else if (Time.time - start > backTime)
            {
                _Dino.animator.SetBool("AttackBox", false);
                _Dino.SetDinoState(new StandingState(_Dino));
                //�ص�ԭʼ״̬������ʵ������������existime�ص�idle��
            }
        }
    }
    IEnumerator OnWaitMethod()
    {
        yield return new WaitForSeconds(time);
        canMove=true;//�پ���time
        start = Time.time;
    }
    //����ֻҪ�ڸ���ʱ���ڰ������ͺ�
    //������һ��attack֮���3s��һ֡���˹�������
}