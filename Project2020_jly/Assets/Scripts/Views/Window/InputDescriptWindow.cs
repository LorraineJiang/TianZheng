using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;

/// <summary>
/// 相机操作说明窗口
/// </summary>
public class InputDescriptWindow : Window
{
    public Button closeButton;
    public Text textContent;
    private InputDescriptViewModel viewModel;
    protected override void OnCreate(IBundle bundle)
    {
        textContent.text = R.view_InputIntroduction_Content;
        viewModel = new InputDescriptViewModel();

        BindingSet<InputDescriptWindow, InputDescriptViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = false;
        MainCameraController.Instance.mIsActive = false;
    }

    public void Close(object sender,InteractionEventArgs args)
    {
        this.Dismiss();
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = true;
        MainCameraController.Instance.mIsActive = true;
    }

    public override void DoDismiss()
    {
        base.DoDismiss();
    }
}
