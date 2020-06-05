using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using UnityEngine;
/// <summary>
/// 相机参数设置窗口的viewmodel
/// </summary>
public class CameraPreferencesViewModel : ViewModelBase
{
    private MainCameraController cameraCtr;
    private DriveCameraController driveCameraCtr;
    private InteractionRequest interClose;
    private SimpleCommand closeCmd;

    public MainCameraController CameraCtr
    {
        get { return this.cameraCtr; }
        set { this.Set<MainCameraController>(ref cameraCtr, value, "CameraCtr"); }
    }
    public DriveCameraController DriveCameraCtr
    {
        get { return this.driveCameraCtr; }
        set { this.Set<DriveCameraController>(ref driveCameraCtr, value, "DriveCameraCtr"); }
    }
    public int CameraRotationSpeed
    {
        get { return this.cameraCtr.mRotationSpeed; }
        set { this.Set<int>(ref cameraCtr.mRotationSpeed, value, "CameraRotationSpeed"); }
    }
    public int CameraScaleSpeed
    {
        get { return this.cameraCtr.mScaleSpeed; }
        set { this.Set<int>(ref cameraCtr.mScaleSpeed, value, "CameraScaleSpeed"); }
    }
    public int CameraMoveSpeed
    {
        get { return this.cameraCtr.mSpeed; }
        set { this.Set<int>(ref cameraCtr.mSpeed, value, "CameraMoveSpeed"); }
    }
    public int DriveCameraRotationSpeed
    {
        get { return this.driveCameraCtr.driveCameraRotationSpeed; }
        set { this.Set<int>(ref driveCameraCtr.driveCameraRotationSpeed, value, "DriveCameraRotationSpeed"); }
    }
    public int DriveCameraMoveSpeed
    {
        get { return this.driveCameraCtr.driveCameraMoveSpeed; }
        set { this.Set<int>(ref driveCameraCtr.driveCameraMoveSpeed, value, "DriveCameraMoveSpeed"); }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public CameraPreferencesViewModel()
    {
        interClose = new InteractionRequest(this);
        closeCmd=new SimpleCommand(()=> {
            this.interClose.Raise();
        });
    }
        
}
