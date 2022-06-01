using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class fenceBroken : MonoBehaviour//√ª∑®”√
{
    // Start is called before the first frame update
    public SkeletonAnimation skeleton;
    void Start()
    {
        //skeleton = GetComponent<SkeletonAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            Debug.Log("Input.GetKey");
            skeleton.state.SetAnimation(0,"smallBroken",true);
        }
    }
}
