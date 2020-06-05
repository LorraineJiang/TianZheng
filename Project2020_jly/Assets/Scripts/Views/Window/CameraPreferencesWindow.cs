using Loxodon.Framework.Commands;
using Loxodon.Framework.Binding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using DG.Tweening;
using Loxodon.Framework.Interactivity;
using UnityStandardAssets.Characters.FirstPerson;
/// <summary>
/// 相机参数设置窗口
/// </summary>
public class CameraPreferencesWindow : Window
{
    #region
    public Slider cameraScaleSpeed;
    public Slider cameraRotationSpeed;
    public Slider cameraMoveSpeed;
    public Slider driveCameraRotationSpeed;
    public Slider driveCameraMoveSpeed;
    public Button closeButton;
    public Text scaleSpeedNum;
    public Text rotationSpeedNum;
    public Text moveSpeedNum;
    public Text driveRotationSpeedNum;
    public Text driveMoveSpeedNum;

    private CameraPreferencesViewModel settingViewModel;
    private IUIViewLocator viewLocator;
    #endregion

    /// <summary>
    /// 初始化数据绑定
    /// </summary>
    /// <param name="bundle"></param>
    protected override void OnCreate(IBundle bundle)
    {
        InitItems();
        //从ApplicationContext应用上下文获得一个视图定位器
        this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
        settingViewModel = new CameraPreferencesViewModel()
        {
            CameraCtr = MainCameraController.Instance,
            DriveCameraCtr=DriveCameraController.Instance
        };
        BindingSet<CameraPreferencesWindow, CameraPreferencesViewModel> bindingSet = this.CreateBindingSet(settingViewModel);
        bindingSet.Bind(this.scaleSpeedNum).For(v => v.text).To(vm => vm.CameraScaleSpeed).OneWay();
        bindingSet.Bind(this.rotationSpeedNum).For(v => v.text).To(vm => vm.CameraRotationSpeed).OneWay();
        bindingSet.Bind(this.moveSpeedNum).For(v => v.text).To(vm => vm.CameraMoveSpeed).OneWay();
        bindingSet.Bind(this.driveRotationSpeedNum).For(v => v.text).To(vm => vm.DriveCameraRotationSpeed).OneWay();
        bindingSet.Bind(this.driveMoveSpeedNum).For(v => v.text).To(vm => vm.DriveCameraMoveSpeed).OneWay();

        bindingSet.Bind(this.cameraScaleSpeed).For(v => v.value, v => v.onValueChanged).To(vm => vm.CameraScaleSpeed).TwoWay();
        bindingSet.Bind(this.cameraRotationSpeed).For(v => v.value, v => v.onValueChanged).To(vm => vm.CameraRotationSpeed).TwoWay();
        bindingSet.Bind(this.cameraMoveSpeed).For(v => v.value, v => v.onValueChanged).To(vm => vm.CameraMoveSpeed).TwoWay();
        bindingSet.Bind(this.driveCameraRotationSpeed).For(v => v.value, v => v.onValueChanged).To(vm => vm.DriveCameraRotationSpeed).TwoWay();
        bindingSet.Bind(this.driveCameraMoveSpeed).For(v => v.value, v => v.onValueChanged).To(vm => vm.DriveCameraMoveSpeed).TwoWay();
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);

        bindingSet.Bind().For(v => v.CloseWindow).To(vm => vm.InterClose);

        bindingSet.Build();

        //屏蔽主相机射线检测（转为检测UI对象且禁止主相机移动）
        MainCameraController.Instance.mIsActiveRaycster = false;
        MainCameraController.Instance.mIsActive = false;
        //若为第一视角移动时还要禁止Rigidbody移动
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
        //JLY
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
    /// 关闭窗口（要释放主相机且若为第一视角时还要释放Rigidbody）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void CloseWindow(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        MainCameraController.Instance.mIsActiveRaycster = true;
        MainCameraController.Instance.mIsActive = true;
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
        //JLY:添加释放驾驶模式相机的功能
        if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
        PlayerPrefs.SetInt("CameraSpeed", (int)cameraMoveSpeed.value);
        PlayerPrefs.SetInt("CameraMoveSpeed", (int)cameraMoveSpeed.value);
        PlayerPrefs.SetInt("CameraRotationSpeed", (int)cameraRotationSpeed.value);
        PlayerPrefs.SetInt("CameraScaleSpeed", (int)cameraScaleSpeed.value);
        PlayerPrefs.SetInt("DriveCameraRotationSpeed", (int)driveCameraRotationSpeed.value);
        PlayerPrefs.SetInt("DriveCameraMoveSpeed", (int)driveCameraMoveSpeed.value);
    }

    /// <summary>
    /// 防止控件没有绑定，用代码绑定一遍
    /// </summary>
    private void InitItems()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>();
        if (sliders.Length > 0)
        {
            if (cameraScaleSpeed == null) cameraScaleSpeed = sliders[0];
            if (cameraRotationSpeed == null) cameraRotationSpeed = sliders[1];
            if (cameraMoveSpeed == null) cameraMoveSpeed = sliders[2];
            if (driveCameraRotationSpeed == null) driveCameraRotationSpeed = sliders[3];
            if (driveCameraMoveSpeed == null) driveCameraMoveSpeed = sliders[4];
        }
        Text[] texts = GetComponentsInChildren<Text>();
        if (texts.Length > 0)
        {
            if (scaleSpeedNum == null) scaleSpeedNum = texts[7];
            if (rotationSpeedNum == null) rotationSpeedNum = texts[8];
            if (moveSpeedNum == null) moveSpeedNum = texts[9];
            if (driveRotationSpeedNum == null) driveRotationSpeedNum = texts[10];
            if (driveMoveSpeedNum == null) driveMoveSpeedNum = texts[11];
        }
    }
}
