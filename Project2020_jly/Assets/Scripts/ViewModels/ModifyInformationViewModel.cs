using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using Loxodon.Framework.Views;
/// <summary>
/// 信息修改窗口的vm
/// </summary>
public class ModifyInformationViewModel : ViewModelBase
{
    private InformationViewModel model;
    private string editorName;      //编辑时的名称
    private string editorInfo;      //编辑时的信息
    private string editorRelativeTable;     //编辑时的关联表名
    private string editorRelativeKey;       //编辑时的关联表主键
    private string editorRelativeKeyValue;      //编辑时的关联主键的值
    private string editorPragma;    //编辑时的参数
    private string editorType;      //编辑时的类型id
    private string editorInterface;     //编辑时的接口字符串；

    private SimpleCommand sureCmd;
    private SimpleCommand cancelCmd;
    private SimpleCommand showTitleNullCmd;     //显示文本错误提示命令
    private InteractionRequest interClose;
    private InteractionRequest<DialogNotification> interTitleEmpty;  //文本错误提示交互请求

    public InteractionRequest<DialogNotification> InterTitleEmpty
    {
        get { return this.interTitleEmpty; }
    }

    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public InformationViewModel Model
    {
        get { return model; }
        set { this.Set<InformationViewModel>(ref model, value, "Model"); }
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

    public string EditorName
    {
        get { return editorName; }
        set { this.Set<string>(ref editorName, value, "EditorName"); }
    }
    public string EditorInfo
    {
        get { return editorInfo; }
        set { this.Set<string>(ref editorInfo, value, "EditorInfo"); }
    }
    public string EditorRelativeTable
    {
        get { return this.editorRelativeTable; }
        set { this.Set<string>(ref editorRelativeTable, value, "EditorRelativeTable"); }
    }
    public string EditorRelativeKey
    {
        get { return this.editorRelativeKey; }
        set { this.Set<string>(ref editorRelativeKey, value, "EditorRelativeKey"); }
    }
    public string EditorRelativeKeyValue
    {
        get { return this.editorRelativeKeyValue; }
        set { this.Set<string>(ref editorRelativeKeyValue, value, "EditorRelativeKeyValue"); }
    }
    public string EditorPragma
    {
        get { return this.editorPragma; }
        set { this.Set<string>(ref editorPragma, value, "EditorPragma"); }
    }
    public string EditorType
    {
        get { return this.editorType; }
        set { this.Set<string>(ref editorType, value, "EditorType"); }
    }
    public string EditorInterface
    {
        get { return this.editorInterface; }
        set { this.Set<string>(ref editorInterface, value, "EditorInterface"); }
    }
    public ModifyInformationViewModel()
    {
        this.sureCmd = new SimpleCommand(() => 
        { 
            Commit();
        });
        this.showTitleNullCmd = new SimpleCommand(() =>
        {
            this.showTitleNullCmd.Enabled = false;

            DialogNotification notification = new DialogNotification(R.application_error, R.view_ModifyInfo_inputTitleNull, R.application_sure, true);
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
        this.cancelCmd = new SimpleCommand(() => { interClose.Raise(); });
        this.interClose = new InteractionRequest(this);
        this.interTitleEmpty = new InteractionRequest<DialogNotification>(this);
    }

    private void Commit()
    {

        if (string.IsNullOrEmpty(editorName))
        {
            this.showTitleNullCmd.Execute(null);
            return;
        }
        else
        {
            Model.Name = editorName;
            Model.Info = editorInfo;
            Model.RelativeTable = editorRelativeTable;
            Model.RelativeKey = editorRelativeKey;
            Model.RelativeKeyValue = editorRelativeKeyValue;
            Model.Pragma = editorPragma;
            Model.Type = editorType;
            Model.RelativeInterface = editorInterface;
            if (SqliteCtr.DbHelper.ExitItem("BaseInfo", "id", model.Id))
            {
                //存在则删除记录
                SqliteCtr.DbHelper.Delete("BaseInfo", new string[] { "id" }, new string[] { model.Id });
            }
            //新增数据
            string[] tempS = new string[10];
            tempS[0] = string.Format("\"{0}\"", Model.RelativeInterface);
            tempS[1] = string.Format("\"{0}\"", Model.Pragma);
            tempS[2] = string.Format("\"{0}\"", Model.Type);
            tempS[3] = string.Format("{0}", Model.Id);
            tempS[4] = string.Format("\"{0}\"", Model.Name);
            tempS[5] = string.Format("\"{0}\"", Model.Info);
            tempS[6] = string.Format("\"{0}\"", Model.Image);
            tempS[7] = string.Format("\"{0}\"", Model.RelativeTable);
            tempS[8] = string.Format("\"{0}\"", Model.RelativeKey);
            tempS[9] = string.Format("\"{0}\"", Model.RelativeKeyValue);
            
            SqliteCtr.DbHelper.InsertInto("BaseInfo", tempS);
            interClose.Raise();
        }
    }
}
