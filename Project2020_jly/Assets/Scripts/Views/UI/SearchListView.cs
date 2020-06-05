using Loxodon.Framework.Binding;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 搜索框中的滚动列表ui
/// </summary>
public class SearchListView : UIView
{
    #region
    public class ItemClickedEvent : UnityEvent<int>
    {
        public ItemClickedEvent()
        {
        }
    }

    private ObservableList<SearchItemModel> items;

    private int countContent;

    public Transform content;

    public GameObject itemTemplate;

    public string chooseScene;

    public ItemClickedEvent OnSelectChanged = new ItemClickedEvent();
    #endregion

    public ObservableList<SearchItemModel> Items
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

    /// <summary>
    /// 动作监听器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
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

    private void AddItem(int newStartingIndex, object v)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 添加所有查找到的数据
    /// </summary>
    public virtual void OnItemsChanged()
    {
        countContent = 0;
        for (int i = 0; i < this.items.Count; i++)
        {
            if(items[i].SceneName == chooseScene)
            {
                this.AddItem(countContent, items[i]);
                countContent++;
            }
        }
    }

    /// <summary>
    /// 搜索结果框中每个结果的显示???
    /// </summary>
    /// <param name="itemViewGo"></param>
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

    /// <summary>
    /// 增删改换
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected virtual void AddItem(int index, SearchItemModel item)
    {
        var itemViewGo = Instantiate(this.itemTemplate);//复制模板Button
        itemViewGo.transform.SetParent(this.content, false);
        itemViewGo.transform.SetSiblingIndex(index);

        Button button = itemViewGo.GetComponent<Button>();
        button.onClick.AddListener(() => OnSelectChange(itemViewGo));
        itemViewGo.SetActive(true);
        Text title = itemViewGo.GetComponentInChildren<Text>();
        title.text = item.Title;    //设置按钮的名称

        //TODO:每个Button是否要设置删除功能
        //Button delButton = itemViewGo.transform.GetChild(1).GetComponent<Button>();
        //delButton.gameObject.SetActive(User.Instance.IsAdmin);  //设置删除按钮权限
        //delButton.onClick.AddListener(
        //    () => {
        //        for (int i = 0; i < content.childCount; i++)
        //        {
        //            if (content.GetChild(i).gameObject == itemViewGo)
        //            {
        //                ShowDeleteMessage(i);   //TODO 弹出删除提示框
        //            }
        //        }
        //    });

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

    /// <summary>
    /// 更换地区后要重新搜索结果，先清空原来的
    /// </summary>
    public void ResetItem()
    {
        for (int i = countContent - 1; i >= 0; i--)
        {
            Transform transform = this.content.GetChild(i);
            Destroy(transform.gameObject);
        }
    }

    /// <summary>
    /// 显示删除节点提示框
    /// </summary>
    /// <param name="index"></param>
    private void ShowDeleteMessage(int index)
    {
        DialogNotification notification = new DialogNotification(R.application_tip, R.view_NavigationList_deletePoint, R.application_sure, R.application_cancel, false);
        //对话框的回调函数
        Action<DialogNotification> callback = n =>
        {
            //this.showTitleNullCmd.Enabled = true;

            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                MessageCenter.Instance.Publish<int>(MessageChannel.RemoveNavigationPoint.ToString(), index);
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
