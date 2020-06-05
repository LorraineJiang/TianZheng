using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using System.Text;
using Loxodon.Framework.Views;
/// <summary>
/// 保存巡航点窗口的vm
/// </summary>
public class SaveNavigationViewModel : ViewModelBase
{
    private CameraPathModel model;
    private SimpleCommand sureCmd;
    private SimpleCommand cancelCmd;
    private SimpleCommand closeCmd;
    private SimpleCommand showTitleNullCmd;     //显示文本错误提示命令
    private SimpleCommand saveSamePathCmd;      //显示同名路径保存命令

    private InteractionRequest interClose;
    private InteractionRequest<DialogNotification> interTitleEmpty;  //文本错误提示交互请求
    private InteractionRequest<DialogNotification> interSaveSamePath;   //同名路径保存请求
    private InteractionRequest<DialogNotification> interCancelTip;  //取消保存请求
    public CameraPathModel Model
    {
        get { return this.model; }
        set { this.Set<CameraPathModel>(ref model, value, "Model"); }
    }

    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public InteractionRequest<DialogNotification> InterCancelTip
    {
        get { return this.interCancelTip; }
    }
    public InteractionRequest<DialogNotification> InterSaveSamePath
    {
        get { return this.interSaveSamePath; }
    }
    public InteractionRequest<DialogNotification> InterTitleEmpty
    {
        get { return this.interTitleEmpty; }
    }

    public ICommand SaveSamePathCmd
    {
        get { return this.saveSamePathCmd; }
    }
    public ICommand ShowTitleNullCmd
    {
        get { return this.showTitleNullCmd; }
    }
    public ICommand SureCmd
    {
        get { return this.sureCmd; }
    }
    public ICommand CancelCmd
    {
        get { return this.cancelCmd; }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }

    public SaveNavigationViewModel()
    {
        this.sureCmd = new SimpleCommand(() => { Commit(); });
        this.closeCmd = new SimpleCommand(() => { interClose.Raise(); });
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
        this.saveSamePathCmd = new SimpleCommand(() =>
          {
              this.saveSamePathCmd.Enabled = false;

              DialogNotification notification = new DialogNotification(R.application_tip, R.view_SaveNavigation_PathExist, R.application_sure,R.application_cancel, true);
              //对话框的回调函数
              Action<DialogNotification> callback = n =>
              {
                  this.saveSamePathCmd.Enabled = true;

                  if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                  {
                      UpdateTable("Navigation");
                  }
                  else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                  {
                      //Debug.LogFormat("Click: No");
                  }
              };
              this.interSaveSamePath.Raise(notification, callback);
          });
        this.cancelCmd = new SimpleCommand(() => {
            this.cancelCmd.Enabled = false;

            DialogNotification notification = new DialogNotification(R.application_tip, R.view_SaveNavigation_CancelEditor, R.application_sure, R.application_cancel, true);
            //对话框的回调函数
            Action<DialogNotification> callback = n =>
            {
                this.cancelCmd.Enabled = true;

                if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                {
                    model.Dispose();
                    model = null;
                    InterClose.Raise();
                    MainCameraController.Instance.isEditorNav = false;
                }
                else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                {

                }
            };
            this.interCancelTip.Raise(notification, callback);
        });

        this.interClose = new InteractionRequest(this);
        this.interTitleEmpty = new InteractionRequest<DialogNotification>(this);
        this.interSaveSamePath = new InteractionRequest<DialogNotification>(this);
        this.interCancelTip = new InteractionRequest<DialogNotification>(this);
    }
    private void Commit()
    {
        //TODO 判断名字是否输入，以及是否已经存在该名字
        if (string.IsNullOrEmpty(model.Name))
        {
            showTitleNullCmd.Execute(null);
            return;
        }
        else
        {
            InsertIntoTable("Navigation");
        }
    }
    /// <summary>
    /// 新增数据到数据库
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="newModel"></param>
    private void InsertIntoTable(string tableName)
    {
        try
        {
            StringBuilder strBuilder = new StringBuilder();

            string[] tempS = new string[5];
            tempS[0] = string.Format("\"{0}\"", Model.Name);
            tempS[1] = string.Format("\"{0}\"", Model.SceneName);
            for (int i = 0; i < Model.Position.Count; i++)
            {
                if (i < Model.Position.Count - 1)
                {
                    strBuilder.Append(Model.Position[i].ToString());
                    strBuilder.Append("|");
                }
                else
                {
                    strBuilder.Append(Model.Position[i].ToString());
                }
            }
            tempS[2] = string.Format("\"{0}\"", strBuilder.ToString());

            strBuilder.Clear();
            for (int i = 0; i < Model.Rotation.Count; i++)
            {
                if (i < Model.Rotation.Count - 1)
                {
                    strBuilder.Append(Model.Rotation[i].ToString());
                    strBuilder.Append("|");
                }
                else
                {
                    strBuilder.Append(Model.Rotation[i].ToString());
                }
            }
            tempS[3] = string.Format("\"{0}\"", strBuilder.ToString());

            tempS[4] = string.Format("\"{0}\"", Model.MoveSpeed.ToString());
            //tempS[5] = string.Format("\"{0}\"", Model.RotationSpeed.ToString());

            if (SqliteCtr.DbHelper.ExitItem(tableName, "name", Model.Name))
            {
                //TODO 数据库中已经存在该路径名时，需要使用更新操作
                this.saveSamePathCmd.Execute(null);
            }
            else
            {
                SqliteCtr.DbHelper.InsertInto(tableName, tempS);
                model.Dispose();
                model = null;
                InterClose.Raise();
                MainCameraController.Instance.isEditorNav = false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 更新数据库数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="model"></param>
    private void UpdateTable(string tableName)
    {
        try
        {
            StringBuilder strBuilder = new StringBuilder();

            string[] tempS = new string[5];
            tempS[0] = string.Format("\"{0}\"", Model.Name);
            tempS[1] = string.Format("\"{0}\"", Model.SceneName);
            for (int i = 0; i < Model.Position.Count; i++)
            {
                if (i < Model.Position.Count - 1)
                {
                    strBuilder.Append(Model.Position[i].ToString());
                    strBuilder.Append("|");
                }
                else
                {
                    strBuilder.Append(Model.Position[i].ToString());
                }
            }
            tempS[2] = string.Format("\"{0}\"", strBuilder.ToString());

            strBuilder.Clear();
            for (int i = 0; i < Model.Rotation.Count; i++)
            {
                if (i < Model.Rotation.Count - 1)
                {
                    strBuilder.Append(Model.Rotation[i].ToString());
                    strBuilder.Append("|");
                }
                else
                {
                    strBuilder.Append(Model.Rotation[i].ToString());
                }
            }
            tempS[3] = string.Format("\"{0}\"", strBuilder.ToString());

            tempS[4] = string.Format("\"{0}\"", Model.MoveSpeed.ToString());
            //tempS[5] = string.Format("\"{0}\"", Model.RotationSpeed.ToString());

            SqliteCtr.DbHelper.UpdateInto(tableName,
                new string[] { "name","scenename", "position", "rotation", "movespeed"/*,"rotationspeed"*/ },
                tempS, "name", tempS[0]);
            model.Dispose();
            model = null;
            InterClose.Raise();
            MainCameraController.Instance.isEditorNav = false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
