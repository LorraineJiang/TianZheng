using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 沙盘场景的相机
/// </summary>
public class AreaCameraController : MonoBehaviour
{
    #region
    private Ray ray;
    private RaycastHit hit;
    private Camera mCamera;
    /// <summary>
    /// 初始的位置
    /// </summary>
    private Vector3 origin;
    private Quaternion originRotation;
    public bool isEnable = true;
    private float mDetaX;
    private float mDetaY;
    public float mRotationSpeed = 4;
    public float minMaxRotation = 60;
    /// <summary>
    /// 场景中的建筑物
    /// </summary>
    private AreaBuidItem[] builds;
    #endregion

    private void Start()
    {
        mCamera = GetComponent<Camera>();
        origin = transform.position;
        originRotation = transform.rotation;
        builds = FindObjectsOfType<AreaBuidItem>();
    }

    /// <summary>
    /// 实时监测
    /// </summary>
    void Update()
    {
        if (!isEnable)
            return;
        if (Physics.Raycast(mCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.collider.GetComponent<HighlighterController>() != null)  //确保鼠标射线发射到的物体是设置为高亮的物体
            {
                hit.collider.GetComponent<HighlighterController>().MouseOver();   //触发highlighter controller中的发光事件
            }

            if (Input.GetMouseButtonDown(0))  //点击事件
            {
                IClickItem click = hit.collider.GetComponent<IClickItem>();
                if (click != null)
                {
                    for (int i = 0; i < builds.Length; i++)
                    {
                        Transform miniWin = builds[i].GetComponent<AreaBuidItem>().miniWindow;   //显示是否进入的小UI窗口
                        miniWin.localScale = new Vector3(0, miniWin.localScale.y, miniWin.localScale.z);
                    }
                    click.ClickCall<AreaCameraController>(this);
                }

            }
        }
        if (Input.GetMouseButton(2))
        {
            CameraRotattion();
        }
    }

    /// <summary>
    /// 返回原始位置
    /// </summary>
    public void MoveToOrigin()
    {
        //DoMove(要移动到的位置,所需的时间)   on开头的为相机移动时DoTween里的动画   DoTween可以连续用.调用n次方法
        transform.DOMove(origin, 1f).OnPlay(() =>
        {
            isEnable = false;
            transform.DORotate(originRotation.eulerAngles, 1f);
        }).OnComplete(() =>
        {
            isEnable = true;
        });
    }

    /// <summary>
    /// 控制相机移动的方向
    /// </summary>
    private void CameraRotattion()
    {
        mDetaX = Input.GetAxis("Mouse X");
        mCamera.transform.Rotate(Vector3.up, mDetaX * mRotationSpeed, Space.World);     //相机水平方向绕世界坐标轴旋转

        mDetaY = Input.GetAxis("Mouse Y");
        float x = ClampEulerAngle((mCamera.transform.localEulerAngles.x - mDetaY * mRotationSpeed),
            -minMaxRotation, minMaxRotation);   //y轴向的旋转
        mCamera.transform.localEulerAngles = new Vector3(x, mCamera.transform.localEulerAngles.y, mCamera.transform.localEulerAngles.z);
    }

    /// <summary>
    /// 控制相机的移动范围
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected float ClampEulerAngle(float value, float min, float max)
    {
        float angle = value - 180;
        if (angle > 0)
        {
            angle -= 180;
        }
        else
        {
            angle += 180;
        }
        return Mathf.Clamp(angle, min, max);  //限制轴向移动范围Mathf.Clamp 限制 value的值在min,max之间,大于max返回max,小于min返回min,否则返回value
    }
}
