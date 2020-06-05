using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;

/// <summary>
/// 巡航路径窗口的viewmodel
/// </summary>
public class NavigationViewModel : ViewModelBase
{
    private readonly ObservableList<NavigationItemModel> items = new ObservableList<NavigationItemModel>();
    private SimpleCommand<int> removeCmd;
    private SimpleCommand closeCmd;
    private InteractionRequest interClose;

    public ObservableList<NavigationItemModel> Items
    {
        get { return this.items; }
    }
    public ICommand RemoveCmd
    {
        get { return this.removeCmd; }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }

    public NavigationViewModel()
    {
        this.interClose = new InteractionRequest(this);
        this.closeCmd = new SimpleCommand(() =>
          {
              interClose.Raise();
          });
        this.removeCmd = new SimpleCommand<int>((index) =>
          {
              RemoveItem(index);
          });
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
                    MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeToFreeCamera.ToString(), true);
                    MessageCenter.Instance.Publish<bool>(MessageChannel.ChangePositionBtnState.ToString(), true);
                    interClose.Raise();
                    if (items[i].SceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
                    {
                        //场景相同时
                        MainCameraController.Instance.StartNavigation(items[i]);
                    }
                    else
                    {
                        //TODO 场景不同时
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
    /// 删除按钮项
    /// </summary>
    /// <param name="index"></param>
    public void RemoveItem(int index)
    {
        if (this.items.Count <= index) return;
        //TODO 删除数据库中的数据
        DeleteTableItem(index, "Navigation");
        this.items.RemoveAt(index);
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
