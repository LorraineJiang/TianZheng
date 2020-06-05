using Loxodon.Framework.Commands;
using Loxodon.Framework.Binding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using DG.Tweening;
using Loxodon.Framework.Interactivity;
using System;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 作业分组窗口
/// </summary>
public class GroupWindow : Window
{
    #region
    //内部的vm
    private GroupViewModel viewModel;
    public GroupViewListView listView;
    private IDisposable closeSubscription;
    public Button addButton;
    public Button closeButton;
    public InputField searchField;
    #endregion

    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new GroupViewModel();
        BindingSet<GroupWindow, GroupViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.addButton).For(v => v.onClick).To(vm => vm.AddCmd);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);

        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Bind().For(v => v.ShowMessage).To(vm => vm.InterSelectNull);

        closeSubscription = MessageCenter.Instance.Subscribe<bool>(MessageChannel.CloseJobGroupWindow.ToString(),
            (b) => { CloseByMessage(b); });

        bindingSet.Build();
        //列表的搜索功能
        searchField.onValueChanged.AddListener(SearchItems);
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = false;
        MainCameraController.Instance.mIsActive = false;
        addButton.gameObject.SetActive(User.Instance.IsAdmin);
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
        //JLY:添加禁止驾驶相机移动的功能
        if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void DoDismiss()
    {
        base.DoDismiss();
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        if (closeSubscription != null)
        {
            closeSubscription.Dispose();
            closeSubscription = null;
        }
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
        //JLY:添加释放驾驶相机移动的功能
        if (ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = true;
        }
    }

    /// <summary>
    /// 当没有选中任何物体时点击“新增”，会弹框显示（Loxodon自带且自动消失）
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

    /// <summary>
    /// 通过消息关闭界面
    /// </summary>
    /// <param name="b"></param>
    private void CloseByMessage(bool b)
    {
        Close(this, null);
    }

    /// <summary>
    /// 搜索按钮功能，属于视图自身的功能
    /// 需要去除最后一个模板按钮不显示
    /// </summary>
    /// <param name="searchFilter"></param>
    public void SearchItems(string searchFilter)
    {
        //若搜索框为空，即什么都不输入的搜索，就显示所有
        if (string.IsNullOrEmpty(searchFilter.Trim()))
        {
            //显示全部
            for (int i = 0; i < listView.content.childCount - 1; i++)
            {
                listView.content.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            //按輸入内容搜索，包含的文本显示
            for (int i = 0; i < listView.content.childCount - 1; i++)
            {
                if (listView.content.GetChild(i).GetComponentInChildren<Text>().text.Contains(searchFilter.Trim()))
                {
                    listView.content.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    listView.content.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
