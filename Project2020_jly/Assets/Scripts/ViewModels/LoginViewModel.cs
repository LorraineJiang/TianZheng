using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Interactivity;
using System;

/// <summary>
/// 登录界面的vm
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private string userName;
    private string passWord;
    private ObservableDictionary<string, string> errorInfo = new ObservableDictionary<string, string>();
    private SimpleCommand loginCmd;
    private InteractionRequest<Notification> toastRequest;
    private InteractionRequest interClose;

    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }

    public InteractionRequest<Notification> ToastReques
    {
        get { return this.toastRequest; }
    }
    public ICommand LoginCmd
    {
        get { return this.loginCmd; }
    }
    public string UserName
    {
        get { return this.userName; }
        set { this.Set<string>(ref userName, value, "UserName"); }
    }
    public string PassWord
    {
        get { return this.passWord; }
        set { this.Set<string>(ref passWord, value, "PassWord"); }
    }
    public ObservableDictionary<string, string> ErrorInfo
    {
        get { return this.errorInfo; }
        set { this.Set<ObservableDictionary<string, string>>(ref errorInfo, value, "ErrorInfo"); }
    }

    public LoginViewModel()
    {
        this.toastRequest = new InteractionRequest<Notification>(this);
        this.interClose = new InteractionRequest(this);
        this.loginCmd = new SimpleCommand(() =>
          {
              Login();
          }
        );
    }

    public void Login()
    {

        if (string.IsNullOrEmpty(userName))
        {
            if (!this.ErrorInfo.ContainsKey("error"))
            {
                this.ErrorInfo.Add("error", R.view_LoginError_userNameNull);
            }
            else
            {
                this.ErrorInfo["error"] = R.view_LoginError_userNameNull;
            }
            toastRequest.Raise(new Notification(this.ErrorInfo["error"]));
            return;
        }

        if (string.IsNullOrEmpty(passWord))
        {
            if (!this.ErrorInfo.ContainsKey("error"))
            {
                this.ErrorInfo.Add("error", R.view_LoginError_passWordNull);
            }
            else
            {
                this.ErrorInfo["error"] = R.view_LoginError_passWordNull;
            }
            toastRequest.Raise(new Notification(this.ErrorInfo["error"]));
            return;
        }
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectByConditions("User", "*", new string[] { "Name", "PassWord" }, new string[] { userName, passWord });
        if (SqliteCtr.Instance.Reader.Read())
        {
            bool te = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("Isadmin")) == "true" ? true : false;
            User.Instance.Init(userName, passWord, te);
            this.ErrorInfo.Clear();
            //进入选择区域场景
            LoxodonWindowCtr.Instance.OpenWindow<LoadingWindow>(Global.prefab_LoadingWindow).LoadScence(Global.scence_shapan);
            this.InterClose.Raise();
        }
        else
        {
            if (!this.ErrorInfo.ContainsKey("error"))
            {
                this.ErrorInfo.Add("error", R.view_LoginError_userNameOrpassWordError);
            }
            else
            {
                this.ErrorInfo["error"] = R.view_LoginError_userNameOrpassWordError;
            }
            toastRequest.Raise(new Notification(this.ErrorInfo["error"]));
            return;
        }

    }
}
