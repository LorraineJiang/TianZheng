using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;

/// <summary>
/// 搜索列表的model
/// </summary>
public class SearchItemModel : ViewModelBase
{
    private string title;
    private bool selected;
    private Vector3 position;
    private string sceneName;

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
    public Vector3 Position
    {
        get { return this.position; }
        set { this.position = value; }
    }
    public string SceneName
    {
        get { return this.sceneName; }
        set { this.Set<string>(ref sceneName, value, "SceneName"); }
    }
}
