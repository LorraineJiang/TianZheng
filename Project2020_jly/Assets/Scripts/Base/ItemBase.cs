using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 物体的基类
/// 可以定义物体的id，是否可以跳转定位，是否可以分组
/// </summary>
public class ItemBase : MonoBehaviour
{
    [Header("物体id")]
    [Tooltip("物体的唯一id，设置时需要不重复")]
    public int id;           
    [Header("物体名称")]
    [Tooltip("中文名称，用于搜索界面的按钮显示使用")]
    public string itemName;
    [Header("所在区域")]
    [Tooltip("该建筑物所在的区域，用于区别搜索场景点")]
    public string atScene;
    [Header("允许定位")]
    [Tooltip("不勾选时，物体没有定位功能")]
    public bool canLocate=true;
    [Header("定位时的偏移量")]
    [Tooltip("搜索定位功能时对位置进行一定偏移，以global坐标为准")]
    public Vector3 locateOffset;  
    [Header("允许分组")]
    [Tooltip("不勾选时，该物体无法进行分组")]
    public bool canGroup;           
}
