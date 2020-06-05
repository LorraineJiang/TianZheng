using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// 沙盘场景中的区域建筑物体
/// </summary>
public class AreaBuidItem : MonoBehaviour,IClickItem
{
    #region
    public Vector3 buildPosition;
    public Vector3 buildRotation;
    private Vector3 dir;
    private Vector3 target;
    private AreaCameraController areaCamera;
    public Transform miniWindow;
    #endregion

    /// <summary>
    /// 重写了IClickItem中的ClickCall方法，加载页面
    /// </summary>
    public void ClickCall()
    {
        //点击物体时，显示加载界面
        LoxodonWindowCtr.Instance.OpenWindow<LoadingWindow>(Global.prefab_LoadingWindow).LoadScence(Global.scence_MainScence, true);
        //场景跳转后的回调
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    /// <summary>
    /// 跳转回调函数
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //进入主场景，修改在主场景中Camera的位置和可移动区域限制
        MainCameraController.Instance.transform.position = buildPosition;
        //Quaternion.Euler函数将Vector类型转为Quaternion类型
        MainCameraController.Instance.transform.rotation = Quaternion.Euler(buildRotation);
        MainCameraController.Instance.ChangeAreaLimit();
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    /// <summary>
    /// 重写Click方法，点击后拉近摄像机到目标并显示小窗
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t1"></param>
    public void ClickCall<T>(T t1)
    {
        if (t1 is AreaCameraController)
        {
            areaCamera = t1 as AreaCameraController;
            if (areaCamera == null) 
                return;
            //确保为沙盘场景的Camera调用的ClickCall方法后，将Camera拉近到target的附近（这里的移动动画用的是DoTween插件里的方法）
            dir = (areaCamera.transform.position - transform.position).normalized;
            target = transform.position + dir * 0.5f;
            target.y = 1.025f;
            areaCamera.transform.DOMove(target, 1f).OnPlay(() =>
            {
                areaCamera.isEnable = false;
                areaCamera.transform.DOLookAt(transform.position, 1f);
            }).OnComplete(()=> {
                //提示是否进入的小窗口在0.2s内比例变化到-1倍
                miniWindow.DOScaleX(-1f, 0.2f);
                Button[] buttons = miniWindow.GetComponentsInChildren<Button>();
                //若点击button后，移除所有listener再添加对应的监听器（0是进入，1是取消）
                buttons[0].onClick.RemoveAllListeners();
                buttons[1].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() => {
                    //若0，则显示加载界面并加载主界面
                    ClickCall();
                });
                buttons[1].onClick.AddListener(() => {
                    //若1，则相机返回至初始位置，且将小窗口在0.2秒内缩小到0倍
                    areaCamera.MoveToOrigin();
                    miniWindow.DOScaleX(0, 0.2f);
                });
                areaCamera.isEnable = true; 
            });
        }
    }
}
