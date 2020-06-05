using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;

/// <summary>
/// 作业分组界面中滚动列表按钮的model
/// </summary>
public class GroupSettingListModel : ViewModelBase
{
    private string title;
    private bool selected;
    private RecordGroupInfoModel groupInfoModel;
    //private ObservableList<int> groupObjectId = new ObservableList<int>();

    public string Title
    {
        get { return this.title; }
        set { this.Set<string>(ref title, value, "Title"); }
    }

    public bool IsSelected
    {
        get { return this.selected; }
        set { this.Set<bool>(ref selected, value, "IsSelected"); }
    }

    //public ObservableList<int> GroupObjectId
    //{
    //    get { return this.groupObjectId; }
    //    set { this.Set<ObservableList<int>>(ref groupObjectId, value, "GroupObjectId"); }
    //}
    public RecordGroupInfoModel GroupInfoModel
    {
        get { return this.groupInfoModel; }
        set { this.Set<RecordGroupInfoModel>(ref groupInfoModel, value, "GroupInfoModel"); }
    }
}
