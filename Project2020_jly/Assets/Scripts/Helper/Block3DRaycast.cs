using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/// <summary>
/// 屏蔽3d物体射线检测的功能类
/// </summary>
public class Block3DRaycast : MonoBehaviour
{
    public static Block3DRaycast Instance;
    EventSystem eventSystem;
    //用于检测投向Canvas的射线
    /*其中：
     *      Ignore Reversed Graphics：忽略颠倒的图形的射线检测，即旋转180°后的图片不会与射线发生交互（检测）
     *      Blocked Objects：会阻挡图形射线的对象类型（2D或(/和)3D对象，需要有collider组件）
     *      Blocked Mask：会阻挡图形射线的Layer
    */
    GraphicRaycaster RaycastInCanvas;
    void Awake()
    {
        Instance = this;
        RaycastInCanvas = this.gameObject.GetComponent<GraphicRaycaster>();
    }
    /// <summary>
    /// 射线检测,如果碰到UI元素则为false,碰到3d物体则为true
    /// </summary>
    /// <returns></returns>
    public bool Is3DObjectsRaycast()
    {
        //按钮点击事件PointerEventData
        PointerEventData eventData = new PointerEventData(eventSystem);
        //pressPosition：按下的时候的指针位置,同样的一次点击事件只有一个
        //pressPosition：当前指针的位置,返回一个vector2向量,这个值是一个屏幕坐标,左下角为原点(0,0),右上角为(屏幕宽,屏幕高)
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        RaycastInCanvas.Raycast(eventData, list);
        return list.Count > 0;
    }
}
