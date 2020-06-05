using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using UnityEngine;
/// <summary>
/// 主界面的viewmodel
/// </summary>
public class MainUIViewModel : ViewModelBase
{
    #region
    private SimpleCommand positionViewOpenCmd;
    private SimpleCommand positionViewCloseCmd;
    private SimpleCommand scenceViewOpenCmd;
    private SimpleCommand groupCmd;
    private SimpleCommand dollyTrackCmd;
    private SimpleCommand jumpPointCmd;
    private SimpleCommand settingCmd;
    private SimpleCommand backCmd;
    private SimpleCommand searchCmd;
    #region 交互请求
    private InteractionRequest interPositionViewShowReq;
    private InteractionRequest interClosePositionReq;
    private InteractionRequest interBackReq;
    private InteractionRequest interSearchReq;

    #endregion
    public ICommand PositionViewCloseCmd
    {
        get { return this.positionViewCloseCmd; }
    }
    public ICommand PositionViewShowCmd
    {
        get { return this.positionViewOpenCmd; }
    }
    public ICommand ScenceViewOpenCmd
    {
        get { return this.scenceViewOpenCmd; }
    }
    public ICommand GroupCmd
    {
        get { return this.groupCmd; }
    }
    public ICommand DollyTrackCmd
    {
        get { return this.dollyTrackCmd; }
    }
    public ICommand JumpPointCmd
    {
        get { return this.jumpPointCmd; }
    }
    public ICommand SettingCmd
    {
        get { return this.settingCmd; }
    }
    public ICommand BackCmd
    {
        get { return this.backCmd; }
    }
    public ICommand SearchCmd
    {
        get { return this.searchCmd; }
    }
    public InteractionRequest InterSearchReq
    {
        get { return this.interSearchReq; }
    }
    public InteractionRequest InterClosePositionViewReq
    {
        get { return this.interClosePositionReq; }
    }
    public InteractionRequest InterPositionViewShowReq
    {
        get { return this.interPositionViewShowReq; }
    }
    public InteractionRequest InterBackReq
    {
        get { return this.interBackReq; }
    }
    private void DollyTrackBtnClick()
    {
        LoxodonWindowCtr.Instance.OpenWindow<NavigationWindow>(Global.prefab_NavigationPathWindow);
    }
    #endregion

    /// <summary>
    /// 定点按钮响应
    /// </summary>
    private void JumpPointBtnClick()
    {
        //主相机激活时
        if (MainCameraController.Instance.gameObject.activeSelf)
        {
            LoxodonWindowCtr.Instance.OpenWindow<SaveJumpPointWindow>(Global.prefab_JumpPointSetting)
                .SetRecordPoint(new RecordPointInfoModel(string.Empty, MainCameraController.Instance.transform.position, MainCameraController.Instance.transform.rotation,UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
        }
        //第一人称相机激活时
        else if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            LoxodonWindowCtr.Instance.OpenWindow<SaveJumpPointWindow>(Global.prefab_JumpPointSetting)
                .SetRecordPoint(new RecordPointInfoModel(string.Empty, ScenceManager.Instance.firstPersonCamer.transform.position, ScenceManager.Instance.firstPersonCamer.transform.rotation,UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
        }
        //驾驶相机激活时
        else if(ScenceManager.Instance.driveCamera.activeSelf)
        {
            LoxodonWindowCtr.Instance.OpenWindow<SaveJumpPointWindow>(Global.prefab_JumpPointSetting)
                .SetRecordPoint(new RecordPointInfoModel(string.Empty, ScenceManager.Instance.driveCamera.transform.position, ScenceManager.Instance.driveCamera.transform.rotation, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name));
        }

    }

    /// <summary>
    /// 打开相机偏好设置窗口
    /// </summary>
    private void SettingBtnClick()
    {
        LoxodonWindowCtr.Instance.OpenWindow<CameraPreferencesWindow>(Global.prefab_SettingWindow);
    }

    /// <summary>
    /// 打开分组设置窗口
    /// </summary>
    private void GroupBtnClick()
    {
        LoxodonWindowCtr.Instance.OpenWindow<GroupWindow>(Global.prefab_GroupSettingWindow);
    }

    public MainUIViewModel()
    {
        Init();
    }

    /// <summary>
    /// 初始化各项内容+在非自由状态时禁用搜索按钮
    /// </summary>
    private void Init()
    {
        #region 交互响应初始化
        this.interPositionViewShowReq = new InteractionRequest(this);
        this.interBackReq = new InteractionRequest(this);
        this.interClosePositionReq = new InteractionRequest(this);
        this.interSearchReq = new InteractionRequest(this);
        #endregion

        #region 命令初始化
        this.positionViewOpenCmd = new SimpleCommand(() =>
        {
            interPositionViewShowReq.Raise();    //调用交互请求
        });
        this.positionViewCloseCmd = new SimpleCommand(() =>
          {
              interClosePositionReq.Raise();
          });
        this.groupCmd = new SimpleCommand(() =>
        {
            GroupBtnClick();
        });
        this.dollyTrackCmd = new SimpleCommand(() =>
        {
            DollyTrackBtnClick();
        });
        this.jumpPointCmd = new SimpleCommand(() =>
          {
              JumpPointBtnClick();
          });
        this.settingCmd = new SimpleCommand(() =>
        {
            SettingBtnClick();
        });
        this.scenceViewOpenCmd = new SimpleCommand(() =>
          {
              LoxodonWindowCtr.Instance.OpenWindow<DifferentViewWindow>(Global.prefab_DifferentViewWindow);
          });
        this.backCmd = new SimpleCommand(() =>
          {
              this.interBackReq.Raise();
          });
        this.searchCmd = new SimpleCommand(() =>
          {
              //第一人称与驾驶模式时禁用搜索按钮
              if (ScenceManager.Instance.firstPersonCamer.activeSelf) return;
              if (ScenceManager.Instance.driveCamera.activeSelf) return;
              this.interSearchReq.Raise();
          });
        #endregion
    }
}
