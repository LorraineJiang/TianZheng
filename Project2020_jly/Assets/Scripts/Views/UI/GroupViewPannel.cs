using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Contexts;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UnityEngine.UI;
using Loxodon.Framework.Views;
/// <summary>
/// 作业分组界面滚动列表的pannel
/// </summary>
public class GroupViewPannel : MonoBehaviour
{
    private GroupViewUIViewModel viewModel;
    private IDisposable addSubscription;
    private IDisposable removeSubscription;
    //滚动列表ui
    public GroupViewListView listView;
    void Start()
    {
        viewModel = new GroupViewUIViewModel();
        try
        {
            ReadItemsFromTable("JobGroup"); //读取数据
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        IBindingContext bindingContext = this.BindingContext();
        bindingContext.DataContext = viewModel;

        //注册新增 作业分组 的消息，消息内部为触发vm的数据新增命令
        addSubscription = MessageCenter.Instance.Subscribe<RecordGroupInfoModel>(MessageChannel.AddJobGroup.ToString(),(n) => { 
            viewModel.AddCmd.Execute(n);
        });
        //注册删除 作业分组 的消息，消息内部为出发vm的数据删除命令
        removeSubscription = MessageCenter.Instance.Subscribe<int>(MessageChannel.RemoveJobGroup.ToString(), (i) => { 
            viewModel.RemoveCmd.Execute(i);
        });

        BindingSet<GroupViewPannel, GroupViewUIViewModel> bindingSet = this.CreateBindingSet<GroupViewPannel, GroupViewUIViewModel>();
        //列表的数据和vm的数据绑定,vm的数据是列表显示的真实数据
        bindingSet.Bind(this.listView).For(v => v.Items).To(vm => vm.Items).OneWay();
        //列表的点击绑定到vm的选择功能上
        bindingSet.Bind(this.listView).For(v => v.OnSelectChanged).To<int>(vm => vm.Select).OneWay();

        bindingSet.Bind().For(v => v.SameNamePopupWindow).To(vm => vm.InterSameGroupName);
        bindingSet.Build();
        ////列表的搜索功能
        //searchInput.onValueChanged.AddListener(SearchItems);
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
    /// <summary>
    /// 读取数据库中的数据
    /// </summary>
    /// <param name="tableName"></param>
    private void ReadItemsFromTable(string tableName)
    {
        List<GroupSettingListModel> groups = new List<GroupSettingListModel>();
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.ReadFullTable(tableName);

        while (SqliteCtr.Instance.Reader.Read())
        {
            //读取每一行数据.并将其变成PositionItemViewModel的形式存储到vm中
            string groupName = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("groupname"));
            string groupContent = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("groupcontent"));
            string sceneName = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("scenename"));
            string position= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("position"));
            string rotation = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("rotation"));
            GroupSettingListModel temp = new GroupSettingListModel() { GroupInfoModel = new RecordGroupInfoModel() };
            temp.Title = groupName;
            temp.GroupInfoModel.Position = PositionAndRotationExtension.ConvertVectr3ByString(position);
            temp.GroupInfoModel.Rotation = PositionAndRotationExtension.ConvertQuaternonByString(rotation);
            temp.GroupInfoModel.SceneName = sceneName;
            if (!string.IsNullOrEmpty(groupContent))
            {
                string[] contents = groupContent.Split(',');
                for (int i = 0; i < contents.Length; i++)
                {
                    temp.GroupInfoModel.GroupObjects.Add(int.Parse(contents[i]));
                }
                groups.Add(temp);
            }
        }
        for (int i = 0; i < groups.Count; i++)
        {
            viewModel.Items.Add(groups[i]);
        }
    }

    /// <summary>
    /// 相同组名时的提示框
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
}
