using Loxodon.Framework.Commands;
using Loxodon.Framework.Binding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Views;
using Loxodon.Framework.Interactivity;

/// <summary>
/// 作业分组保存界面
/// </summary>
public class SaveGroupsWindow : Window
{
    private SaveGroupsViewModel viewModel;

    public Button sureButton;
    public Button closeButton;
    public InputField nameInput;

    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new SaveGroupsViewModel();

        BindingSet<SaveGroupsWindow, SaveGroupsViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind(this.sureButton).For(v => v.onClick).To(vm => vm.SureCmd);
        bindingSet.Bind(this.nameInput).For(v => v.text, v => v.onEndEdit).To(vm => vm.EditorGroupName).TwoWay();
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Bind().For(v => v.ShowTitleEmptyMessage).To(vm => vm.InterNameTip);
        bindingSet.Build();
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActive = false;
        MainCameraController.Instance.mIsActiveRaycster = false;
    }
    public override void DoDismiss()
    {
        base.DoDismiss();
    }

    private void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
    }

    public void SetModel(RecordGroupInfoModel info)
    {
        viewModel.GroupInfo = info;
    }
    private void ShowTitleEmptyMessage(object sender, InteractionEventArgs args)
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
