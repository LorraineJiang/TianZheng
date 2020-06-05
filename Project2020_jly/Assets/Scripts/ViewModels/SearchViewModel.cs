using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using System;

/// <summary>
/// 搜索窗口对应的viewmodel
/// </summary>
public class SearchViewModel : ViewModelBase
{
    private ObservableList<SearchItemModel> items = new ObservableList<SearchItemModel>();
    private SimpleCommand<int> removeCmd;
    private SimpleCommand closeCmd;
    private InteractionRequest interClose;

    public ObservableList<SearchItemModel> Items
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

    public SearchViewModel()
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
                    ScenceManager.Instance.SearchItem(items[i].Title);
                    interClose.Raise();
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
        this.items.RemoveAt(index);
    }
}
