using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using System;
using Loxodon.Framework.Views;

/// <summary>
/// 作业分组提示界面的viewmodel
/// </summary>
public class SaveGroupsViewModel : ViewModelBase
{
    private string editorGroupName;
    private RecordGroupInfoModel groupInfo;
    private SimpleCommand sureCmd;
    private SimpleCommand closeCmd;
    private SimpleCommand nameIsNullCmd;

    private InteractionRequest interClose;
    private InteractionRequest<DialogNotification> interNameTip;

    public string EditorGroupName
    {
        get { return editorGroupName; }
        set { Set<string>(ref editorGroupName, value, "EditorGroupName"); }
    }
    public RecordGroupInfoModel GroupInfo
    {
        get { return this.groupInfo; }
        set { this.Set<RecordGroupInfoModel>(ref groupInfo, value, "GroupInfo"); }
    }

    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public InteractionRequest<DialogNotification> InterNameTip
    {
        get { return this.interNameTip; }
    }
    public ICommand NameIsNullCmd
    {
        get { return this.nameIsNullCmd; }
    }
    public ICommand SureCmd
    {
        get { return this.sureCmd; }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }


    public SaveGroupsViewModel()
    {
        this.sureCmd = new SimpleCommand(() => { SureBtnClick(); });
        this.closeCmd = new SimpleCommand(() => { interClose.Raise(); });
        this.nameIsNullCmd = new SimpleCommand(() =>
        {
            DialogNotification notification = new DialogNotification(R.application_tip, R.view_settingPosition_inputNameNull, R.application_sure, true);
            Action<DialogNotification> callback = (n) =>
              {
                  if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                  {
                      //Debug.LogFormat("Click: Yes");
                  }
                  else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                  {
                      //Debug.LogFormat("Click: No");
                  }
              };
            this.interNameTip.Raise(notification, callback);
        }
        );
        this.interClose = new InteractionRequest(this);
        this.interNameTip = new InteractionRequest<DialogNotification>(this);
    }

    public void SureBtnClick()
    {
        if (string.IsNullOrEmpty(editorGroupName))
        {
            this.nameIsNullCmd.Execute(null);
        }
        else
        {
            this.GroupInfo.Name = editorGroupName;
            //TODO 判断是否已经存在组名了？
            MessageCenter.Instance.Publish<RecordGroupInfoModel>(MessageChannel.AddJobGroup.ToString(), this.groupInfo);
            this.closeCmd.Execute(null);
        }
    }
}
