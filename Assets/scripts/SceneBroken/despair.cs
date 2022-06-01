using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class despair : MonoBehaviour//生成之后定时消失(打错了应该是disappear。。。
{
    // Start is called before the first frame update
    public float interval = 0.5f;
    public float startTime = 0;
    public float curTime=0;
    void Start()
    {
        curTime = Time.time;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        curTime = Time.time;
        if ((curTime - startTime) > interval)
        {
            Destroy(gameObject);
        }
    }
}
