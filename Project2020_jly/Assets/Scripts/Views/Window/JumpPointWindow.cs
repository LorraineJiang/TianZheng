using System.Collections.Generic;
using UnityEngine;
using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UnityEngine.UI;
using Loxodon.Framework.Views;

/// <summary>
/// 按钮滚动列表UI，内部是按钮
/// </summary>
public class JumpPointWindow : MonoBehaviour
{
    //滚动列表的vm
    private JumpPointListItemViewModel viewModel;
    private IDisposable addSubscription;
    private IDisposable removeSubscription;
    //滚动列表
    public JumpPointListView listView;
    //搜索文本框
    public InputField searchInput;
    public Button closeButton;

    private void Start()
    {
        InitItems();
        viewModel = new JumpPointListItemViewModel();
        try
        {
            ReadItemsFromTable("RecordPoint");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        IBindingContext bindingContext = this.BindingContext();
        bindingContext.DataContext = viewModel;

        //注册新增记录点的消息，消息内部为触发vm的数据新增命令
        addSubscription = MessageCenter.Instance.Subscribe<RecordPointInfoModel>(MessageChannel.AddPositionPoint.ToString(), (m) => { viewModel.AddCmd.Execute(m); });
        //注册删除记录点的消息，消息内部为出发vm的数据删除命令
        removeSubscription = MessageCenter.Instance.Subscribe<int>(MessageChannel.RemovePositionPoint.ToString(), (i) => { viewModel.RemoveCmd.Execute(i); });
       
        BindingSet<JumpPointWindow, JumpPointListItemViewModel> bindingSet = this.CreateBindingSet<JumpPointWindow, JumpPointListItemViewModel>();
        //列表的数据和vm的数据绑定,vm的数据是列表显示的真实数据
        bindingSet.Bind(this.listView).For(v => v.Items).To(vm => vm.Items).OneWay();
        //列表的点击绑定到vm的选择功能上
        bindingSet.Bind(this.listView).For(v => v.OnSelectChanged).To<int>(vm => vm.Select).OneWay();
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        //列表弹出提示框
        bindingSet.Bind().For(v => v.SameNamePopupWindow).To(vm => vm.InterSameName);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Build();
        //列表的搜索功能
        searchInput.onValueChanged.AddListener(SearchItems);
        this.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (addSubscription != null)
        {
            addSubscription.Dispose();
            addSubscription = null;
        }
        if (removeSubscription != null)
        {
            removeSubscription.Dispose();
            removeSubscription = null;
        }
    }
    public void Close(object sender, EventArgs arg)
    {
        MessageCenter.Instance.Publish<bool>(MessageChannel.ClosePositionWindow.ToString(), true);
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
            for (int i = 0; i < listView.content.childCount-1; i++)
            {
                listView.content.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            //按輸入内容搜索，包含的文本显示
            for (int i = 0; i < listView.content.childCount-1; i++)
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

    /// <summary>
    /// 读取表中的数据来填充vm中的items
    /// </summary>
    /// <param name="tableName">数据表名</param>
    private void ReadItemsFromTable(string tableName)
    {
        List<JumpPointItemModel> positions = new List<JumpPointItemModel>();
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.ReadFullTable(tableName);
        while (SqliteCtr.Instance.Reader.Read())
        {
            //读取每一行数据.并将其变成PositionItemViewModel的形式存储到vm中
            string title = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
            string vector = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("position"));
            string rotation = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("rotation"));
            string sceneName= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("scenename"));
            JumpPointItemModel temp = new JumpPointItemModel();
            temp.Title = title;
            temp.Position = PositionAndRotationExtension.ConvertVectr3ByString(vector);
            temp.Rotation = PositionAndRotationExtension.ConvertQuaternonByString(rotation);
            temp.SceneName = sceneName;
            positions.Add(temp);
            //创建地图图标
            try
            {
                //场景名相同时创建对应的图标
                if (temp.SceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                {
                    ScenceManager.Instance.CreateMiniMapIcon(temp.Title, temp.Position, temp.Rotation);
                }
                else 
                { 

                }
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
            
        }
        for (int i = 0; i < positions.Count; i++)
        {
            viewModel.Items.Add(positions[i]);
        }
    }
    /// <summary>
    /// 同名提示弹窗
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void SameNamePopupWindow(object sender, InteractionEventArgs args)
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

    private void InitItems()
    {
        if (listView == null)
            listView = GetComponentInChildren<JumpPointListView>();
        if (searchInput == null)
            searchInput = GetComponentInChildren<InputField>();
    }
}
