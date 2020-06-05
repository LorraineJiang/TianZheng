using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using Loxodon.Framework.Views;

/// <summary>
/// 建筑信息界面的viewmodel
/// </summary>
public class InformationViewModel : ViewModelBase
{
    private string id;
    private string name;
    private string info;    //介绍内容
    private string image;
    private string relativeTable;   //关联表名
    private string relativeKey;     //关联表主键(暂未使用)
    private string relativeKeyValue;    //关联的主键的值（暂未使用）
    private string type;       //模型类型
    private string pragma;      //模型参数（可以手动输入的参数）
    private string relativeInterface;   //模型关联接口字符串（掉用远端的sql语句）

    private SimpleCommand closeCmd;
    private SimpleCommand modifyCmd;
    private SimpleCommand clearCmd;
    private SimpleCommand enterCmd;
    private SimpleCommand setImgCmd;
    private SimpleCommand clearImgCmd;
    private InteractionRequest interSetImg;
    private InteractionRequest interClearImg;
    private InteractionRequest interClose;
    private InteractionRequest interClear;
    private InteractionRequest interEnter;
    private InteractionRequest<DialogNotification> interClearDialog;

    public ICommand SetImgCmd
    {
        get { return this.setImgCmd; }
    }
    public ICommand ClearImgCmd
    {
        get { return this.clearImgCmd; }
    }
    public ICommand EnterCmd
    {
        get { return this.enterCmd; }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public ICommand ModifyCmd
    {
        get { return this.modifyCmd; }
    }
    public ICommand ClearCmd
    {
        get { return this.clearCmd; }
    }
    public InteractionRequest InterEnter
    {
        get { return this.interEnter; }
    }
    public InteractionRequest InterSetImg
    {
        get { return this.interSetImg; }
    }
    public InteractionRequest InterClearImg
    {
        get { return this.interClearImg; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public InteractionRequest InterClear
    {
        get { return this.interClear; }
    }
    public InteractionRequest<DialogNotification> InterClearDialog
    {
        get { return this.interClearDialog; }
    }
    public string Id
    {
        get { return this.id; }
        set { this.Set<string>(ref id, value, "Id"); }
    }
    public string Name
    {
        get { return this.name; }
        set { this.Set<string>(ref name, value, "Name"); }
    }
    public string Info
    {
        get { return this.info; }
        set { this.Set<string>(ref info, value, "Info"); }
    }
    public string Image
    {
        get { return this.image; }
        set { this.Set<string>(ref image, value, "Image"); }
    }
    public string RelativeTable
    {
        get { return this.relativeTable; }
        set { this.Set<string>(ref relativeTable, value, "RelativeTable"); }
    }
    public string RelativeKey
    {
        get { return this.relativeKey; }
        set { this.Set<string>(ref relativeKey, value, "RelativeKey"); }
    }
    public string RelativeKeyValue
    {
        get { return this.relativeKeyValue; }
        set { this.Set<string>(ref relativeKeyValue, value, "RelativeKeyValue"); }
    }
    public string Type
    {
        get { return this.type; }
        set { this.Set<string>(ref type, value, "Type"); }
    }
    public string Pragma
    {
        get { return this.pragma; }
        set { this.Set<string>(ref pragma, value, "Pragma"); }
    }
    public string RelativeInterface
    {
        get { return this.relativeInterface; }
        set { this.Set<string>(ref relativeInterface, value, "RelativeInterface"); }
    }

    public InformationViewModel()
    {
        this.interClose = new InteractionRequest(this);
        this.interClear = new InteractionRequest(this);
        this.interClearDialog = new InteractionRequest<DialogNotification>(this);
        this.interSetImg = new InteractionRequest(this);
        this.interClearImg = new InteractionRequest(this);
        this.interEnter = new InteractionRequest(this);
        this.closeCmd = new SimpleCommand(() =>
          {
              this.interClose.Raise();
          });
        this.modifyCmd = new SimpleCommand(() =>
          {
              LoxodonWindowCtr.Instance.OpenWindow<ModifyInformationWindow>(Global.prefab_ModifyInfoWindow).SetModel(this);
          });
        this.clearCmd = new SimpleCommand(() =>
          {
              if (string.IsNullOrEmpty(this.name))
                  return;

              DialogNotification notification = new DialogNotification(R.application_tip, "是否删除数据", R.application_sure, R.application_cancel, false);
              //对话框的回调函数
              Action<DialogNotification> callback = n =>
              {
                  if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                  {
                      this.interClear.Raise();
                  }
                  else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                  {
                  }
              };
              this.interClearDialog.Raise(notification, callback);
          });
        this.enterCmd = new SimpleCommand(() =>
          {
              this.interEnter.Raise();
          });
        this.setImgCmd = new SimpleCommand(() =>
        {
            this.interSetImg.Raise();
        });
        this.clearImgCmd = new SimpleCommand(() =>
        {
            this.interClearImg.Raise();
        });
    }
}
