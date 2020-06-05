using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击响应的接口
/// </summary>
public interface IClickItem
{
    void ClickCall();
    void ClickCall<T>(T t1);
}
