using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using System;
using Loxodon.Framework.Views;

/// <summary>
/// 兴趣点列表中按钮的vm
/// </summary>
public class JumpPointListItemViewModel : ViewModelBase
{
    #region
    private ObservableList<JumpPointItemModel> items = new ObservableList<JumpPointItemModel>();
    private SimpleCommand<RecordPointInfoModel> addCmd;
    private SimpleCommand<int> removeCmd; 
    private SimpleCommand clearCmd;
    private SimpleCommand closeCmd;
    private SimpleCommand<JumpPointItemModel> showSamePointName;

    private InteractionRequest<DialogNotification> interSameName;
    private InteractionRequest interClose;
    //用于记录同名位置点对应的下标
    private int sameNameIndex;
    public InteractionRequest<DialogNotification> InterSameName
    {
        get { return this.interSameName; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public ICommand ShowSamePositionName
    {
        get { return this.showSamePointName; }
    }
    public ICommand AddCmd
    {
        get { return this.addCmd; }
    }
    public ICommand RemoveCmd
    {
        get { return this.removeCmd; }
    }
    public ICommand ClearCmd
    {
        get { return this.clearCmd; } 
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public ObservableList<JumpPointItemModel> Items
    {
        get { return this.items; }
        set { this.Set<ObservableList<JumpPointItemModel>>(ref items, value, "Items"); }
    }

    public JumpPointItemModel SelectedItem
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

    public JumpPointListItemViewModel()
    {
        InitCmd();
        this.interSameName = new InteractionRequest<DialogNotification>(this);
        this.interClose = new InteractionRequest(this);
    }

    /// <summary>
    /// 添加元素，先判断是否有相同名称的点，存在则替换，反之则新增
    /// </summary>
    /// <param name="recordInfo">记录点信息类</param>
    public void AddItem(RecordPointInfoModel recordInfo)
    {
        JumpPointItemModel model = new JumpPointItemModel() 
        {
            Title = recordInfo.Title, Position = recordInfo.Position, 
            Rotation = recordInfo.Rotation, SceneName = recordInfo.SceneName 
        };
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Title == recordInfo.Title)
            {
                //当标记点重名时，记录数组下标位置
                sameNameIndex = i;
                //TODO 弹出提示消息
                this.showSamePointName.Execute(model);
                return;
            }
        }
        // 如果不包含该title的数据，则直接添加
        this.items.Add(model);
        //TODO 创建图标，新增数据库
        InsertIntoTable("RecordPoint", model);
        ScenceManager.Instance.CreateMiniMapIcon(model.Title, model.Position, model.Rotation);
        MessageCenter.Instance.Publish<bool>(MessageChannel.CloseSaveJumpPointWindow.ToString(), true);
    }
    public void AddItem()
    {
        this.items.Add(new JumpPointItemModel() { Title = "title " });
    }

    /// <summary>
    /// 删除元素
    /// </summary>
    /// <param name="index">元素的下标</param>
    public void RemoveItem(int index)
    {
        if (this.items.Count <= index) return;
        ScenceManager.Instance.RemoveMiniMapIcon(this.items[index].Title);
        //TODO 删除数据库中的数据
        DeleteTableItem(index, "RecordPoint");
        this.items.RemoveAt(index);
    }

    /// <summary>
    /// 清空元素
    /// </summary>
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
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == items[i].SceneName)
                    {
                        //Debug.Log("场景相同");
                        MainCameraController.Instance.JumpTo(items[i].Position, items[i].Rotation,
                            true,()=> { MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true); });
                        MessageCenter.Instance.Publish<bool>(MessageChannel.ClosePositionWindow.ToString(), true);
                        
                    }
                    else
                    {
                        //Debug.Log("场景不相同");
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

    /// <summary>
    /// 初始化命令
    /// </summary>
    private void InitCmd()
    {
        this.addCmd = new SimpleCommand<RecordPointInfoModel>((info) =>
        {
            AddItem(info);
        });
        this.removeCmd = new SimpleCommand<int>((index) =>
          {
              RemoveItem(index);
          });
        this.closeCmd = new SimpleCommand(() =>
        {
            //关闭窗口且释放主场景相机
            this.interClose.Raise();
              MainCameraController.Instance.mIsActive = true;
              MainCameraController.Instance.mIsActiveRaycster = true;
              MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true);
          });
        this.showSamePointName = new SimpleCommand<JumpPointItemModel>((model) =>
        {
            {
                this.showSamePointName.Enabled = false;

                DialogNotification notification = new DialogNotification(R.application_tip, R.view_positionListItem_replaceCurrentPoint, R.application_sure, R.application_cancel, false);
                //对话框的回调函数
                Action<DialogNotification> callback = n =>
                {
                    this.showSamePointName.Enabled = true;

                    if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
                    {
                        //TODO 移除原先的坐标点重新创建
                        ScenceManager.Instance.ReplaceMiniMapIcon(model.Title, model.Position, model.Rotation);
                        UpdateTable("RecordPoint", model);
                        MessageCenter.Instance.Publish<bool>(MessageChannel.CloseSaveJumpPointWindow.ToString(), true);
                        this.Items[sameNameIndex] = model;  //替换item元素
                    }
                    else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)//Loxodon的AlertDialog的取消按键自带关闭窗口功能
                    {
                    }
                };
                this.interSameName.Raise(notification, callback);
            }
        });
    }

    /// <summary>
    /// 新增数据到数据库
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="newModel"></param>
    private void InsertIntoTable(string tableName, JumpPointItemModel newModel)
    {
        try
        {
            string[] tempS = new string[4];
            tempS[0] = string.Format("\"{0}\"", newModel.Title);
            tempS[1] = string.Format("\"{0}\"", newModel.SceneName);
            tempS[2] = string.Format("\"{0}\"", newModel.Position.ToString());
            tempS[3] = string.Format("\"{0}\"", newModel.Rotation.ToString());
            SqliteCtr.DbHelper.InsertInto(tableName, tempS);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 更新数据库数据
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="model"></param>
    private void UpdateTable(string tableName, JumpPointItemModel model)
    {
        try
        {;
            //更新数据库（SQL语句）
            string[] tempS = new string[4];
            tempS[0] = string.Format("\"{0}\"", model.Title);
            tempS[1] = string.Format("\"{0}\"", model.SceneName);
            tempS[2] = string.Format("\"{0}\"", model.Position.ToString());
            tempS[3] = string.Format("\"{0}\"", model.Rotation.ToString());
            SqliteCtr.DbHelper.UpdateInto(tableName,
                new string[] { "name","scenename", "position","rotation" },
                tempS, "name", tempS[0]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// 删除数据库数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tableName"></param>
    private void DeleteTableItem(int index, string tableName)
    {
        try
        {
            string col = string.Format("\"{0}\"", this.items[index].Title);
            SqliteCtr.DbHelper.Delete(tableName, new string[] { "name" }, new string[] { col });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
