﻿using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 作业分组界面的滚动列表ui
/// </summary>
public class GroupViewListView : UIView
{
    public class ItemClickedEvent : UnityEvent<int>
    {
        public ItemClickedEvent()
        {
        }
    }

    private ObservableList<GroupSettingListModel> items;

    public Transform content;

    public GameObject itemTemplate;

    public ItemClickedEvent OnSelectChanged = new ItemClickedEvent();

    public ObservableList<GroupSettingListModel> Items
    {
        get { return this.items; }
        set
        {
            if (this.items == value)
                return;

            if (this.items != null)
                this.items.CollectionChanged -= OnCollectionChanged;

            this.items = value;

            this.OnItemsChanged();

            if (this.items != null)
                this.items.CollectionChanged += OnCollectionChanged;
        }
    }

    protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                this.AddItem(eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                break;
            case NotifyCollectionChangedAction.Remove:
                this.RemoveItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0]);
                break;
            case NotifyCollectionChangedAction.Replace:
                this.ReplaceItem(eventArgs.OldStartingIndex, eventArgs.OldItems[0], eventArgs.NewItems[0]);
                break;
            case NotifyCollectionChangedAction.Reset:
                this.ResetItem();
                break;
            case NotifyCollectionChangedAction.Move:
                this.MoveItem(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex, eventArgs.NewItems[0]);
                break;
        }
    }
    protected virtual void OnItemsChanged()
    {
        for (int i = 0; i < this.items.Count; i++)
        {
            this.AddItem(i, items[i]);
        }
    }
    protected virtual void OnSelectChange(GameObject itemViewGo)
    {
        if (this.OnSelectChanged == null || itemViewGo == null)
            return;

        for (int i = 0; i < this.content.childCount; i++)
        {
            var child = this.content.GetChild(i);
            if (itemViewGo.transform == child)
            {
                this.OnSelectChanged.Invoke(i);
                break;
            }
        }
    }
    protected virtual void AddItem(int index, object item)
    {
        var itemViewGo = Instantiate(this.itemTemplate);
        itemViewGo.transform.SetParent(this.content, false);
        itemViewGo.transform.SetSiblingIndex(index);

        Button button = itemViewGo.GetComponent<Button>();
        button.onClick.AddListener(() => OnSelectChange(itemViewGo));
        itemViewGo.SetActive(true);
        Text title = itemViewGo.GetComponentInChildren<Text>();
        title.text = items[index].Title;    //设置按钮的名称

        Button delButton = itemViewGo.transform.GetChild(1).GetComponent<Button>();
        delButton.gameObject.SetActive(User.Instance.IsAdmin);
        delButton.onClick.AddListener(
            () => {
                for (int i = 0; i < content.childCount; i++)
                {
                    if (content.GetChild(i).gameObject == itemViewGo)
                    {
                        ShowRemoveTipMessage(i);   //弹出删除提示框
                    }
                }
            });

    }

    protected virtual void RemoveItem(int index, object item)
    {
        Transform transform = this.content.GetChild(index);
        UIView itemView = transform.GetComponent<UIView>();
        if (itemView != null)
        {
            if (itemView.GetDataContext() == item)
            {
                itemView.gameObject.SetActive(false);
                Destroy(itemView.gameObject);
            }
        }
        else
        {
            Destroy(transform.gameObject);
        }
    }

    protected virtual void ReplaceItem(int index, object oldItem, object item)
    {
        Transform transform = this.content.GetChild(index);
        UIView itemView = transform.GetComponent<UIView>();
        if (itemView.GetDataContext() == oldItem)
        {
            itemView.SetDataContext(item);
        }
    }

    protected virtual void MoveItem(int oldIndex, int index, object item)
    {
        Transform transform = this.content.GetChild(oldIndex);
        UIView itemView = transform.GetComponent<UIView>();
        itemView.transform.SetSiblingIndex(index);
    }

    protected virtual void ResetItem()
    {
        for (int i = this.content.childCount - 1; i >= 0; i--)
        {
            Transform transform = this.content.GetChild(i);
            Destroy(transform.gameObject);
        }
    }
    /// <summary>
    /// 删除节点的提示弹窗
    /// </summary>
    /// <param name="index"></param>
    private void ShowRemoveTipMessage(int index)
    {
        DialogNotification notification = new DialogNotification(R.application_tip, R.view_GroupViewList_deleteCurrentGroup, R.application_sure, R.application_cancel, false);
        //对话框的回调函数
        Action<DialogNotification> callback = n =>
        {
            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                MessageCenter.Instance.Publish<int>(MessageChannel.RemoveJobGroup.ToString(), index);
            }
            else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
            {
            }
        };

        //显示消息框
        AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
        {
            notification.DialogResult = result;
            if (callback != null)
                callback(notification);
        });
    }
}