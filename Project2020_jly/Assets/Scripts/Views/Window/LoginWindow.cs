using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using Loxodon.Framework.Interactivity;
/// <summary>
/// 登录窗口
/// </summary>
public class LoginWindow : Window
{
    public InputField userNameInput;
    public InputField passWordInput;
    public Text errorInfo;
    public Button loginBtn;
    public Button quitBtn;
    private LoginViewModel viewModel;


    protected override void OnCreate(IBundle bundle)
    {
        InitItems();
        viewModel = new LoginViewModel();
        BindingSet<LoginWindow, LoginViewModel> bindingset = this.CreateBindingSet(viewModel);

        bindingset.Bind(userNameInput).For(v => v.text, v => v.onEndEdit).To(vm => vm.UserName).TwoWay();
        bindingset.Bind(passWordInput).For(v => v.text, v => v.onEndEdit).To(vm => vm.PassWord).TwoWay();
        bindingset.Bind(errorInfo).For(v => v.text).To(vm => vm.ErrorInfo["error"]).OneWay();
        bindingset.Bind(loginBtn).For(v => v.onClick).To(vm => vm.LoginCmd);
        
        bindingset.Bind().For(v => v.ShowMessage).To(vm => vm.ToastReques);
        bindingset.Bind().For(v => v.CloseWindow).To(vm => vm.InterClose);
        bindingset.Build();

        quitBtn.onClick.AddListener(() => { Application.Quit(); });
    }
    public override void DoDismiss()
    {
        base.DoDismiss();
    }
    /// <summary>
    /// 消息弹框
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ShowMessage(object sender, InteractionEventArgs args)
    {
        Notification notification = args.Context as Notification;
        if (notification == null)
            return;
        Toast.Show(this, notification.Message, 2f);
    }

    public void CloseWindow(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        //Application.Quit();
    }

    /// <summary>
    /// 组件缺失时自动初始化
    /// </summary>
    private void InitItems()
    {
        InputField[] inputs = GetComponentsInChildren<InputField>();
        if (inputs.Length >= 2)
        {
            if (userNameInput == null)
                userNameInput = inputs[0];
            if (passWordInput == null)
                passWordInput = inputs[1];
        }
        Text[] texts = GetComponentsInChildren<Text>();
        if (texts.Length >= 0)
        {
            if (errorInfo == null)
                errorInfo = texts[6];
        }
        Button[] buttons = GetComponentsInChildren<Button>();
        if (buttons.Length > 0)
        {
            if (loginBtn == null)
                loginBtn = buttons[0];
            if (quitBtn == null)
                quitBtn = buttons[1];
        }
}
}
