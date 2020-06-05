using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using System;
using Loxodon.Framework.Views;
using System.Text;

/// <summary>
/// 作业组界面的滚动列表所对应的vm
/// </summary>
public class GroupViewUIViewModel : ViewModelBase
{
    #region
    private readonly ObservableList<GroupSettingListModel> items = new ObservableList<GroupSettingListModel>();
    private SimpleCommand<RecordGroupInfoModel> addCmd;
    private SimpleCommand<int> removeCmd;
    private SimpleCommand<GroupSettingListModel> sameGroupNameCmd;
    private int sameIndex;
    private InteractionRequest<DialogNotification> interSameGroupName;
    public InteractionRequest<DialogNotification> InterSameGroupName
    {
        get { return this.interSameGroupName; }
    }
    public ICommand SameGroupNameCmd
    {
        get { return this.sameGroupNameCmd; }
    }
    public ICommand AddCmd
    {
        get { return this.addCmd; }
    }
    public ICommand RemoveCmd
    {
        get { return this.removeCmd; }
    }

    public ObservableList<GroupSettingListModel> Items
    {
        get { return this.items; }
    }
    public GroupSettingListModel SelectedItem
    {
        get
        {
            foreach (var item in items)
            {
                if (item.IsSelected)
                    return item;
            }
            return null;
        }
    }
    #endregion

    public void AddItem(RecordGroupInfoModel groupInfo)
    {
        GroupSettingListModel newModel = new GroupSettingListModel()
        {
            GroupInfoModel = groupInfo,
            Title = groupInfo.Name
        };
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Title == newModel.Title)
            {
                sameIndex = i;
                this.sameGroupNameCmd.Execute(newModel); //当组名相同时记录该节点
                return;
            }
        }
        this.items.Add(newModel);

        //TODO 添加新数据到数据库
        InsertIntoTable("JobGroup", newModel);
    }
    public void AddItem()
    {
        this.items.Add(new GroupSettingListModel() { Title = "title " });
    }

    public void RemoveItem(int index)
    {
        if (this.items.Count <= index) return;

        //TODO 删除数据库中的数据
        DeleteTableItem(index, "JobGroup");
        this.items.RemoveAt(index);
    }

    public void ClearItem()
    {
        if (this.items.Count <= 0)
            return;

        this.items.Clear();
    }
    public void Select(int index)
    {
        if (index <= -1 || index > this.items.Count - 1)
            return;

        for (int i = 0; i < this.items.Count; i++)
        {
            if (i == index)
            {
                items[i].IsSelected = !items[i].IsSelected;
                if (items[i].IsSelected)
                {
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != items[i].GroupInfoModel.SceneName)
                    {
                        //Debug.Log("跳转场景");
                    }
                    else
                    {
                        MainCameraController.Instance.JumpTo(items[i].GroupInfoModel.Position, items[i].GroupInfoModel.Rotation,
                            true,()=> { MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true); });
                        ScenceManager.Instance.HighlightByObjList(items[i].GroupInfoModel.GroupObjects);
                        MessageCenter.Instance.Publish<bool>(MessageChannel.CloseJobGroupWindow.ToString(), true);
                    }
                }
                else
                {
                }
            }
            else
            {
                items[i].IsSelected = false;
            }
        }
    }
    public GroupViewUIViewModel()
    {
        addCmd = new SimpleCommand<RecordGroupInfoModel>((group) => 
        {
            AddItem(group);
        });
        removeCmd = new SimpleCommand<int>((index) => 
        {
            RemoveItem(index);
        });
        sameGroupNameCmd = new SimpleCommand<GroupSettingListModel>((model) =>
          {
              this.sameGroupNameCmd.Enabled = false;

              DialogNotification notification = new DialogNotification(R.application_tip, R.view_GroupSetting_groupNameExist, R.application_sure, R.application_cancel, false);
              //对话框的回调函数
              Action<DialogNotification> callback = n =>
              {
                  this.sameGroupNameCmd.Enabled = true;

                  if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                  {
                      //TODO 更新数据库中的对应数据
                      UpdateTable("JobGroup", model);
                      this.items[sameIndex] = model;  //替换item元素
                  }
                  else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
                  {
                  }
              };
              this.interSameGroupName.Raise(notification, callback);
          }
        );
        interSameGroupName = new InteractionRequest<DialogNotification>(this);
    }
    /// <summary>
    /// 新增数据库中的数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="newModel"></param>
    private void InsertIntoTable(string tableName, GroupSettingListModel newModel)
    {
        try
        {
            string[] tempS = new string[5];
            tempS[0] = string.Format("\"{0}\"", newModel.Title);
            tempS[1] = string.Format("\"{0}\"", newModel.GroupInfoModel.SceneName);
            tempS[2] = string.Format("\"{0}\"", newModel.GroupInfoModel.Rotation.ToString());
            tempS[3] = string.Format("\"{0}\"", newModel.GroupInfoModel.Position.ToString());
            StringBuilder stringB = new StringBuilder();
            for (int j = 0; j < newModel.GroupInfoModel.GroupObjects.Count; j++)
            {
                stringB.Append(newModel.GroupInfoModel.GroupObjects[j]);
                if (j != newModel.GroupInfoModel.GroupObjects.Count - 1)
                    stringB.Append(",");
            }
            tempS[4] = string.Format("\"{0}\"", stringB.ToString());
            SqliteCtr.DbHelper.InsertInto(tableName, tempS);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    /// <summary>
    /// 更新数据库中的数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="model"></param>
    private void UpdateTable(string tableName,GroupSettingListModel model)
    {
        try
        {
            string[] tempS = new string[5];
            tempS[0] = string.Format("\"{0}\"", model.Title);
            tempS[1] = string.Format("\"{0}\"", model.GroupInfoModel.SceneName);
            tempS[2] = string.Format("\"{0}\"", model.GroupInfoModel.Rotation.ToString());
            tempS[3] = string.Format("\"{0}\"", model.GroupInfoModel.Position.ToString());
            StringBuilder stringB = new StringBuilder();
            for (int j = 0; j < model.GroupInfoModel.GroupObjects.Count; j++)
            {
                stringB.Append(model.GroupInfoModel.GroupObjects[j]);
                if (j != model.GroupInfoModel.GroupObjects.Count - 1)
                    stringB.Append(",");
            }
            tempS[4] = string.Format("\"{0}\"", stringB.ToString());
            SqliteCtr.DbHelper.UpdateInto(tableName,
                new string[] { "groupname","scenename","rotation","position", "groupcontent" },
                tempS, "groupname", tempS[0]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    /// <summary>
    /// 删除库中的数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tableName"></param>
    private void DeleteTableItem(int index,string tableName)
    {
        try
        {
            string col = string.Format("\"{0}\"", this.items[index].Title);
            SqliteCtr.DbHelper.Delete(tableName, new string[] { "groupname" }, new string[] { col });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
