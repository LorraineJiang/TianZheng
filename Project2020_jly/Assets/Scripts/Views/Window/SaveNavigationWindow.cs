using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
/// <summary>
/// 保存巡航点的窗口
/// </summary>
public class SaveNavigationWindow : Window
{
    private SaveNavigationViewModel viewModel;
    public InputField nameInput;
    public Slider moveSpeedSlider;
    public Button sureButton;
    public Button cancelButton;
    public Button closeButton;
    public Text moveSpeedText;
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new SaveNavigationViewModel();
        BindingSet<SaveNavigationWindow, SaveNavigationViewModel> bindingSet = this.CreateBindingSet(viewModel);

        bindingSet.Bind(this.nameInput).For(v => v.text, v => v.onEndEdit).To(vm => vm.Model.Name).TwoWay();
        bindingSet.Bind(this.moveSpeedSlider).For(v => v.value, v => v.onValueChanged).To(vm => vm.Model.MoveSpeed).TwoWay();
        bindingSet.Bind(this.moveSpeedText).For(v => v.text).To(vm => vm.Model.MoveSpeed).OneWay();
        bindingSet.Bind(this.sureButton).For(v => v.onClick).To(vm => vm.SureCmd);
        bindingSet.Bind(this.cancelButton).For(v => v.onClick).To(vm => vm.CancelCmd);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind().For(v=>v.ShowTipWindow).To(vm=>vm.InterTitleEmpty);
        bindingSet.Bind().For(v => v.ShowTipWindow).To(vm => vm.InterSaveSamePath);
        bindingSet.Bind().For(v => v.ShowTipWindow).To(vm => vm.InterCancelTip);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        MainCameraController.Instance.mIsActive = false;
        MainCameraController.Instance.mIsActiveRaycster = false;
    }

    public override void DoDismiss()
    {
        base.DoDismiss();
    }
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        MainCameraController.Instance.mIsActive = true;
        MainCameraController.Instance.mIsActiveRaycster = true;
    }
    public void SetModel(CameraPathModel model)
    {
        viewModel.Model = model;
    }
    /// <summary>
    /// 巡航路径名为空的消息显示
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ShowTipWindow(object sender, InteractionEventArgs args)
    {
        DialogNotification notification = args.Context as DialogNotification;
        var callback = args.Callback;

        if (notification == null)
            return;
        //显示消息框
        AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
        {
            notification.DialogResult = result;
            //点击对应选项后的回调函数，该回调在vm中
            if (callback != null)
                callback();
        });
    }
}
