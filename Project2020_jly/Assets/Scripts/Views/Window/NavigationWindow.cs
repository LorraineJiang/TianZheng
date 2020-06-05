using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Views;
using UnityEngine.UI;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using System;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 巡航窗口
/// </summary>
public class NavigationWindow : Window
{
    private NavigationViewModel viewModel;
    public InputField searchInput;
    public Button closeButton;
    public NavigationListView listView;
    private IDisposable removeSubscription;
    private IDisposable changeFreeSubscription;
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new NavigationViewModel();
        try
        {
            ReadItemsFromTable("Navigation");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        BindingSet<NavigationWindow, NavigationViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind(this.listView).For(v => v.Items).To(vm => vm.Items).OneWay();
        bindingSet.Bind(this.listView).For(v => v.OnSelectChanged).To<int>(vm => vm.Select).OneWay();
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        //列表的搜索功能
        searchInput.onValueChanged.AddListener(SearchItems);
        removeSubscription = MessageCenter.Instance.Subscribe<int>(MessageChannel.RemoveNavigationPoint.ToString(), (i) => { viewModel.RemoveCmd.Execute(i); });
        changeFreeSubscription = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangeToFreeCamera.ToString(), (b) =>
           {
               ChangeViewToMain(b);
           });
        MainCameraController.Instance.mIsActive = false;
        MainCameraController.Instance.mIsActiveRaycster = false;
        if (ScenceManager.Instance.firstPersonCamer.activeSelf)
        {
            ScenceManager.Instance.firstPersonCamer.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
        //JLY:添加禁止驾驶相机移动的功能
        if(ScenceManager.Instance.driveCamera.activeSelf)
        {
            ScenceManager.Instance.driveCamera.GetComponent<RigidbodyFirstPersonController>().enabled = false;
        }
    }
    public override void DoDismiss()
    {
        base.DoDismiss();
        if (removeSubscription != null)
        {
            removeSubscription.Dispose();
            removeSubscription = null;
        }
        if (changeFreeSubscription != null)
        {
            changeFreeSubscription.Dispose();
            changeFreeSubscription = null;
        }
    }
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        MainCameraController.Instance.mIsActive = true;
        MainCameraController.Instance.mIsActiveRaycster = true;
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
    /// 读取表中的数据来填充vm中的items
    /// </summary>
    /// <param name="tableName">数据表名</param>
    private void ReadItemsFromTable(string tableName)
    {
        //List<NavigationViewModel> positions = new List<NavigationViewModel>();
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.ReadFullTable(tableName);
        while (SqliteCtr.Instance.Reader.Read())
        {
            //读取每一行数据.并将其变成PositionItemViewModel的形式存储到vm中
            string title = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
            string position = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("position"));
            string rotation = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("rotation"));
            float moveSpeed= SqliteCtr.Instance.Reader.GetFloat(SqliteCtr.Instance.Reader.GetOrdinal("movespeed"));
            //float rotationSpeed= SqliteCtr.Instance.Reader.GetFloat(SqliteCtr.Instance.Reader.GetOrdinal("rotationspeed"));
            string sceneName= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("scenename"));
            string[] tempPosition = position.Split('|');
            string[] tempRotation = rotation.Split('|');
            NavigationItemModel tempModel = new NavigationItemModel();
            tempModel.Title = title;
            for (int i = 0; i < tempPosition.Length; i++)
            {
                tempModel.Position.Add(PositionAndRotationExtension.ConvertVectr3ByString(tempPosition[i]));
            }
            for (int i = 0; i < tempRotation.Length; i++)
            {
                tempModel.Rotation.Add(PositionAndRotationExtension.ConvertQuaternonByString(tempRotation[i]));
            }
            tempModel.MoveSpeed = moveSpeed;
            //tempModel.RotationSpeed = rotationSpeed;
            tempModel.SceneName = sceneName;
            viewModel.Items.Add(tempModel);
            tempModel.Dispose();
            tempModel = null;
        }
    }

    /// <summary>
    /// 切换主相机视角
    /// </summary>
    public void ChangeViewToMain(bool state)
    {
        //此处不管驾驶模式，因为巡航状态开的是主相机
        MainCameraController.Instance.gameObject.SetActive(state);
        ScenceManager.Instance.firstPersonCamer.SetActive(false);
        ScenceManager.Instance.driveCamera.SetActive(false);
        ScenceManager.Instance?.miniMap.SetTarget(MainCameraController.Instance.gameObject);
        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = MainCameraController.Instance.gameObject.GetComponent<Camera>();
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
