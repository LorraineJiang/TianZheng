using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Views;
using UnityEngine.UI;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using System;

/// <summary>
/// 搜索窗口
/// </summary>
public class SearchWindow : Window
{
    private SearchViewModel viewModel;
    private Boolean flag;
    public InputField searchInput;
    public Button closeButton;
    public SearchListView listView;
    public Dropdown selectScene;
    
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new SearchViewModel();
        InitModel();
        BindingSet<SearchWindow, SearchViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind(this.listView).For(v => v.Items).To(vm => vm.Items).OneWay();
        bindingSet.Bind(this.listView).For(v => v.OnSelectChanged).To<int>(vm => vm.Select).OneWay();
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        //场景的搜索功能
        selectScene.onValueChanged.AddListener(changeOption);
        //列表的搜索功能
        searchInput.onValueChanged.AddListener(SearchItems);
        MainCameraController.Instance.mIsActive = false;
        MainCameraController.Instance.mIsActiveRaycster = false;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void DoDismiss()
    {
        base.DoDismiss();
    }

    /// <summary>
    /// 关闭窗口并释放相机
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        MainCameraController.Instance.mIsActive = true;
        MainCameraController.Instance.mIsActiveRaycster = true;
    }

    /// <summary>
    /// 若可定位，则初始化数据并加进Model里，来跳转
    /// </summary>
    private void InitModel()
    {
        flag = false;
        for (int i = 0; i < ScenceManager.Instance.Items.Length; i++)
        {
            if (ScenceManager.Instance.Items[i].canLocate)
            {
                SearchItemModel tempModel = new SearchItemModel();
                tempModel = new SearchItemModel();
                tempModel.Title = ScenceManager.Instance.Items[i].itemName;
                tempModel.Position = ScenceManager.Instance.Items[i].gameObject.transform.position;
                tempModel.SceneName = ScenceManager.Instance.Items[i].atScene;
                viewModel.Items.Add(tempModel);
                tempModel.Dispose();
                tempModel = null;
            }
        }
        changeOption(0);
    }

    private void changeOption(int index)
    {
        listView.chooseScene = selectScene.options[index].text;
        if(flag)
        {
            listView.ResetItem();
            listView.OnItemsChanged();
        }
        SearchItems("");
        flag = true;
    }

    /// <summary>
    /// 搜索按钮功能，属于视图自身的功能
    /// 需要去除最后一个模板按钮不显示
    /// </summary>
    /// <param name="searchFilter"></param>
    public void SearchItems(string searchFilter)
    {
        if (viewModel.Items.Count <= 0) return;

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
