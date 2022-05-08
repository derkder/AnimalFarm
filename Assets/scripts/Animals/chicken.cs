using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chicken : BaseAnimal
{
    void Start()
    {
        speed = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))//µÚ¶þÁ¬»÷
        {
            Debug.Log("test");
        }
        speed = 5;
    }
    /*
    public override void MoveToTarget(Vector3 target)
    {

    }*/
}
