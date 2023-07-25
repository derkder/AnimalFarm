//-------------------------------------------------------------------------------------
//	DinoBaseState.cs
//-------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public interface DinoBaseState
{
     //这里取名叫update其实不是非常准确，其实只是方法里面的东西实现了一次
    void Update();
    void HandleInput();

}
