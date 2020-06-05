using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using Loxodon.Framework.Views;

/// <summary>
/// 设置跳转点界面的viewmodel类
/// </summary>
public class SaveJumpPointViewModel : ViewModelBase
{
    private SimpleCommand sureCmd;
    private SimpleCommand cancelCmd;
    private SimpleCommand showTitleNullCmd;     //显示文本错误提示命令

    private InteractionRequest interClose;
    private InteractionRequest<DialogNotification> interTitleEmpty;  //文本错误提示交互请求

    private RecordPointInfoModel recordInfo;

    public RecordPointInfoModel RecordInfo
    {
        get { return this.recordInfo; }
        set { this.Set<RecordPointInfoModel>(ref recordInfo, value, "RecordInfo"); }
    }

    public InteractionRequest<DialogNotification> InterTitleEmpty
    {
        get { return this.interTitleEmpty; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }

    public ICommand SureCmd
    {
        get { return this.sureCmd; }
    }
    public ICommand CancelCmd
    {
        get { return this.cancelCmd; }
    }
    public ICommand ShowTitleNullCmd
    {
        get { return this.showTitleNullCmd; }
    }
    public void OnSureBtnClick()
    {
        if (string.IsNullOrEmpty(this.recordInfo.Title.Trim()))
        {
            showTitleNullCmd.Execute(null);     //当输入的标题为空时，调用错误提示命令
            return;
        }
        MessageCenter.Instance.Publish<RecordPointInfoModel>(MessageChannel.AddPositionPoint.ToString(), this.recordInfo);
    }
    public void OnCancelBtnClick()
    {
        interClose.Raise();
    }

    public SaveJumpPointViewModel()
    {
        this.sureCmd = new SimpleCommand(() => { OnSureBtnClick(); });
        this.cancelCmd = new SimpleCommand(() => { OnCancelBtnClick(); });
        //title为空时的对话框
        this.showTitleNullCmd = new SimpleCommand(() =>
          {
              this.showTitleNullCmd.Enabled = false;

              DialogNotification notification = new DialogNotification(R.application_error, R.view_settingPosition_inputNameNull, R.application_sure, true);
              //对话框的回调函数
              Action<DialogNotification> callback = n =>
              {
                  this.showTitleNullCmd.Enabled = true;

                  if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                  {
                      //Debug.LogFormat("Click: Yes");
                  }
                  else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                  {
                      //Debug.LogFormat("Click: No");
                  }
              };
              this.interTitleEmpty.Raise(notification, callback);
          }
        );
        this.interClose = new InteractionRequest(this);
        this.interTitleEmpty = new InteractionRequest<DialogNotification>(this);
    }
}
