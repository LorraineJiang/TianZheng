using System;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 视角切换的窗口
/// </summary>
public class DifferentViewWindow : Window
{
    #region
    private IDisposable freeViewSubscription;
    private IDisposable firstViewSubscription;
    private IDisposable driveViewSubscription;
    public Button changeToFirstButtons;
    public Button changeToMainButtons;
    public Button changeToDriveButtons;
    public Button closeButton;
    private DifferentViewModel viewModel;
    //TODO 其他视角相机物体
    #endregion

    /// <summary>
    /// 初始化数据及方法绑定
    /// </summary>
    /// <param name="bundle"></param>
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new DifferentViewModel();
        BindingSet<DifferentViewWindow, DifferentViewModel> bindingSet = this.CreateBindingSet(viewModel);
        //注册对应的消息
        freeViewSubscription = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangeToFreeCamera.ToString(), (b) => { MainCameraView(); });
        firstViewSubscription = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangeToFirstCamera.ToString(), (b) => { FirstCameraView(); });
        driveViewSubscription = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangeToDriveCamera.ToString(), (b) => { DriveCameraView(); });

        bindingSet.Bind(this.changeToMainButtons).For(v => v.onClick).To(vm => vm.ChangeToMainCmd);
        bindingSet.Bind(this.changeToFirstButtons).For(v => v.onClick).To(vm => vm.ChangeToFirstCmd);
        bindingSet.Bind(this.changeToDriveButtons).For(v => v.onClick).To(vm => vm.ChangeToDriveCmd);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind().For(v => v.ChangeToMainView).To(vm => vm.InterChangeToMain);
        bindingSet.Bind().For(v => v.ChangeToFirstView).To(vm => vm.InterChangeToFirst);
        bindingSet.Bind().For(v => v.ChangeToDriveView).To(vm => vm.InterChangeToDrive);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = false;
        MainCameraController.Instance.mIsActive = false;
        //若为第一视角移动时还要禁止Rigidbody移动
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
        //JLY:添加禁止驾驶相机移动的功能
        if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void DoDismiss()
    {
        base.DoDismiss();
    }

    /// <summary>
    /// 关闭窗口，释放注册的资源（要释放主相机且若为第一视角时还要释放Rigidbody）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void Close(object sender, EventArgs args)
    {
        this.Dismiss();
        if (freeViewSubscription != null)
        {
            freeViewSubscription.Dispose();
            freeViewSubscription = null;
        }
        if (firstViewSubscription != null)
        {
            firstViewSubscription.Dispose();
            firstViewSubscription = null;
        }
        if(driveViewSubscription != null)
        {
            driveViewSubscription.Dispose();
            driveViewSubscription = null;
        }
        MainCameraController.Instance.mIsActiveRaycster = true;
        MainCameraController.Instance.mIsActive = true;
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
        //JLY:添加释放驾驶相机移动的功能
        if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
    }

    /// <summary>
    /// 频道中发布消息切换为自由视角且自动关闭本窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ChangeToMainView(object sender, EventArgs args)
    {
        MessageCenter.Instance.Publish<bool>(MessageChannel.ScenceViewChangeBtn.ToString(), true);
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeToFreeCamera.ToString(), true);
        this.Close(null, null);
    }

    /// <summary>
    /// 频道中发布消息切换为第一视角且自动关闭本窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ChangeToFirstView(object sender, EventArgs args)
    {
        //此处的false、true是在切换不同视角时能否跳转兴趣点
        MessageCenter.Instance.Publish<bool>(MessageChannel.ScenceViewChangeBtn.ToString(), false);
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeToFirstCamera.ToString(), true);
        this.Close(null,null);
    }

    /// <summary>
    /// 频道中发布消息切换为驾驶视角且自动关闭本窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ChangeToDriveView(object sender,EventArgs args)
    {
        //jly:不确定在驾驶模式时能否跳转兴趣点
        MessageCenter.Instance.Publish<bool>(MessageChannel.ScenceViewChangeBtn.ToString(), false);
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeToDriveCamera.ToString(), true);
        this.Close(null, null);
    }

    /// <summary>
    /// 切换自由视角
    /// </summary>
    private void MainCameraView()
    {
        MainCameraController.Instance.gameObject?.SetActive(true);
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            //当第一人称相机激活时，设置自由相机位置和角度，然后隐藏第一人称相机
            MainCameraController.Instance.gameObject.transform.position = new Vector3(
                ScenceManager.Instance.firstPersonCamer.transform.position.x,
                (ScenceManager.Instance.firstPersonCamer.transform.position.y < MainCameraController.Instance.minPosition.y) ?
                MainCameraController.Instance.minPosition.y : ScenceManager.Instance.firstPersonCamer.transform.position.y + 1,
                ScenceManager.Instance.firstPersonCamer.transform.position.z
                );
            MainCameraController.Instance.gameObject.transform.rotation = ScenceManager.Instance.firstPersonCamer.transform.rotation;
            ScenceManager.Instance.firstPersonCamer.SetActive(false);
        }
        else if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            //当驾驶相机激活时，设置自由相机位置和角度，然后隐藏驾驶相机
            MainCameraController.Instance.gameObject.transform.position = new Vector3(
                ScenceManager.Instance.driveCamera.transform.position.x,
                (ScenceManager.Instance.driveCamera.transform.position.y < MainCameraController.Instance.minPosition.y) ?
                MainCameraController.Instance.minPosition.y : ScenceManager.Instance.driveCamera.transform.position.y + 1,
                ScenceManager.Instance.driveCamera.transform.position.z
                );
            MainCameraController.Instance.gameObject.transform.rotation = ScenceManager.Instance.driveCamera.transform.rotation;
            ScenceManager.Instance.driveCamera.SetActive(false);
        }
        //设置小地图锁定自由相机
        ScenceManager.Instance?.miniMap.SetTarget(MainCameraController.Instance.gameObject);
        //设置小地图的四个方向朝向
        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = MainCameraController.Instance.gameObject.GetComponent<Camera>();
    }

    /// <summary>
    /// 切换第一人称视角
    /// </summary>
    private void FirstCameraView()
    {
        ScenceManager.Instance?.firstPersonCamer?.SetActive(true);
        if (MainCameraController.Instance.gameObject.activeSelf)
        {
            //当自由相机激活时，设置第一人称相机位置和角度，隐藏自由相机
            ScenceManager.Instance.firstPersonCamer.transform.position = CheckHitPos(MainCameraController.Instance.gameObject.transform.position,(Vector3.up*2));
            MainCameraController.Instance.gameObject.SetActive(false);
        }
        else if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            //当驾驶相机激活时，设置第一人称相机位置和角度，隐藏驾驶相机
            ScenceManager.Instance.firstPersonCamer.transform.position = CheckHitPos(ScenceManager.Instance.driveCamera.transform.position, (Vector3.up * 2));
            ScenceManager.Instance.driveCamera.SetActive(false);
        }
        //设置小地图锁定第一视角相机
        ScenceManager.Instance?.miniMap.SetTarget(ScenceManager.Instance?.firstPersonCamer);
        //设置小地图的四个方向朝向
        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = ScenceManager.Instance?.firstPersonCamer.GetComponentInChildren<Camera>();
    }

    /// <summary>
    /// 切换驾驶视角
    /// </summary>
    private void DriveCameraView()
    {
        ScenceManager.Instance?.driveCamera?.SetActive(true);
        if(MainCameraController.Instance.gameObject.activeSelf)
        {
            //当自由相机激活时，设置驾驶相机位置和角度，隐藏自由相机
            ScenceManager.Instance.driveCamera.transform.position = CheckHitPos(MainCameraController.Instance.gameObject.transform.position, (Vector3.up * 10));
            //ScenceManager.Instance.driveCamera.transform.position = new Vector3(MainCameraController.Instance.transform.position.x,
            //    DriveCameraController.Instance.rotationY,MainCameraController.Instance.transform.position.z);
            MainCameraController.Instance.gameObject.SetActive(false);
        }
        else if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            //当第一视角相机激活时，设置驾驶相机位置和角度，隐藏第一视角相机
            ScenceManager.Instance.driveCamera.transform.position = CheckHitPos(ScenceManager.Instance.firstPersonCamer.transform.position, (Vector3.up * 10));
            //ScenceManager.Instance.driveCamera.transform.position = new Vector3(ScenceManager.Instance.firstPersonCamer.transform.position.x,
            //    DriveCameraController.Instance.rotationY, ScenceManager.Instance.transform.position.z);
            ScenceManager.Instance.firstPersonCamer.SetActive(false);
        }
        //设置小地图锁定驾驶相机
        ScenceManager.Instance?.miniMap.SetTarget(ScenceManager.Instance?.driveCamera);
        //设置小地图的四个方向朝向
        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = ScenceManager.Instance?.driveCamera.GetComponentInChildren<Camera>();
    }

    /// <summary>
    /// 检测可碰撞点的位置，用来切换第一人称时使用
    /// </summary>
    /// <param name="originPos">当前位置点</param>
    /// <param name="offset">位置点的偏移量</param>
    /// <returns></returns>
    private Vector3 CheckHitPos(Vector3 originPos,Vector3 offset)
    {
        Ray tempUpRay = new Ray(originPos, Vector3.up);
        Ray tempDownRay = new Ray(originPos, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(tempDownRay, out hit))
        {

            return hit.point + offset;
        }
        else if (Physics.Raycast(tempUpRay, out hit))
        {
            return hit.point + offset;
        }
        else
            return originPos;
    }
}
