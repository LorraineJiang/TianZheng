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
/// 加载界面窗口
/// </summary>
public class LoadingWindow : Window
{
    public Slider progressBar;
    public Text progressBarNum;

    private IUIViewLocator viewLocator;
    private LoadingViewModel viewModel;
    protected override void OnCreate(IBundle bundle)
    {
        InitItems();
        this.viewLocator = Context.GetApplicationContext().GetService<IUIViewLocator>();
        viewModel = new LoadingViewModel() { };

        BindingSet<LoadingWindow, LoadingViewModel> bindingset = this.CreateBindingSet(viewModel);

        //绑定加载进度条,同时这是激活状态和enable相关，已经进度数字显示
        bindingset.Bind(this.progressBar).For(v => v.value, v => v.onValueChanged).To(vm => vm.ProgressBar.Progress).TwoWay();
        bindingset.Bind(this.progressBar.gameObject).For(v => v.activeSelf).To(vm => vm.ProgressBar.Enable).OneWay();
        bindingset.Bind(this.progressBarNum).For(v => v.text).ToExpression(vm => string.Format("{0}%", Mathf.FloorToInt(vm.ProgressBar.Progress * 100f))).OneWay();

        bindingset.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingset.Build();

    }
    public override void DoDismiss()
    {
        base.DoDismiss();
    }
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
    }
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="scenceName">场景名</param>
    /// <param name="isOpenMainUI">是否显示主UI</param>
    public void LoadScence(string scenceName, bool isOpenMainUI = false)
    {
        viewModel.ScenceName = scenceName;
        viewModel.IsOpenMainUI = isOpenMainUI;
        viewModel.LoadingCmd.Execute(null);
    }
    /// <summary>
    /// 组件为空时初始化组件
    /// </summary>
    private void InitItems()
    {
        if (progressBar == null)
            progressBar = GetComponentInChildren<Slider>();
        if (progressBarNum == null)
            progressBarNum = GetComponentInChildren<Text>();
    }
}
