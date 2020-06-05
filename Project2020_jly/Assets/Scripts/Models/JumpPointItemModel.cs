using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
/// <summary>
/// 兴趣点滚动列表中按钮的model
/// </summary>
public class JumpPointItemModel : ViewModelBase
{
    private string title;
    private bool selected;
    private Vector3 position;
    private Quaternion rotation;
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
        set { this.Set<Vector3>(ref position, value, "Position"); }
    }

    public Quaternion Rotation
    {
        get { return this.rotation; }
        set { this.Set<Quaternion>(ref rotation, value, "Rotation"); }
    }
    public string SceneName
    {
        get { return this.sceneName; }
        set { this.Set<string>(ref sceneName, value, "SceneName"); }
    }
}
