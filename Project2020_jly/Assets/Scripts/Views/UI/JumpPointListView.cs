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
/// 兴趣点的滚动列表ui
/// </summary>
public class JumpPointListView : UIView
{
    public class ItemClickedEvent : UnityEvent<int>
    {
        public ItemClickedEvent()
        {
        }
    }

    private ObservableList<JumpPointItemModel> items;

    public Transform content;

    public GameObject itemTemplate;

    public ItemClickedEvent OnSelectChanged = new ItemClickedEvent();

    /// <summary>
    /// 在ListView里bindingSet中加载数据时被调用
    /// </summary>
    public ObservableList<JumpPointItemModel> Items
    {
        get { return this.items; }
        set
        {
            if (this.items == value)
                return;

            //此处的-、+是因为担心item有旧值，所以-先注销，赋值value之后再用+调用新的
            if (this.items != null)
                this.items.CollectionChanged -= OnCollectionChanged;

            this.items = value;

            this.OnItemsChanged();

            if (this.items != null)
                this.items.CollectionChanged += OnCollectionChanged;
        }
    }

    /// <summary>
    /// 根据事件类型的不同调用不同的函数
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

    /// <summary>
    /// 利用for循环实现每一个跳转点的增加与显示
    /// </summary>
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

    /// <summary>
    /// 跳转点的显示（包含对跳转点按键右上角删除按键的监听）
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected virtual void AddItem(int index, object item)
    {
        var itemViewGo = Instantiate(this.itemTemplate);
        //false则不修改修改父对象的相对位置、比例和旋转
        itemViewGo.transform.SetParent(this.content, false);
        //设置新节点在其兄弟节点中的位置（后移）
        itemViewGo.transform.SetSiblingIndex(index);

        Button button = itemViewGo.GetComponent<Button>();
        button.onClick.AddListener(() => OnSelectChange(itemViewGo));
        itemViewGo.SetActive(true);
        Text title = itemViewGo.GetComponentInChildren<Text>();
        title.text = items[index].Title;    //设置按钮中Text的名称

        Button delButton = itemViewGo.transform.GetChild(1).GetComponent<Button>();
        delButton.gameObject.SetActive(User.Instance.IsAdmin);  //设置删除按钮权限
        delButton.onClick.AddListener(
            () => {
                for (int i = 0; i < content.childCount; i++)
                {
                    if (content.GetChild(i).gameObject == itemViewGo)
                    {
                        ShowDeleteMessage(i);   //弹出提示框
                    }
                }
            });

    }

    //下方的RemoveItem、ReplaceItem、MoveItem、ResetItem为框架中的，在实际中应当使用不到这些功能
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
        //由于在AddItem函数里后移的功能，此时排在第一个的按键的下标为最大值，所以此处i要倒回去
        for (int i = this.content.childCount - 1; i >= 0; i--)
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
        DialogNotification notification = new DialogNotification(R.application_tip, R.view_positionList_deletePoint, R.application_sure,R.application_cancel, false);
        //对话框的回调函数
        Action<DialogNotification> callback = n =>
        {
            //this.showTitleNullCmd.Enabled = true;

            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                MessageCenter.Instance.Publish<int>(MessageChannel.RemovePositionPoint.ToString(), index);
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
