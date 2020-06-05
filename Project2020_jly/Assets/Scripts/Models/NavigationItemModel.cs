using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;
/// <summary>
/// 巡航路径滚动列表的model，包含有巡航的数据
/// </summary>
public class NavigationItemModel : ViewModelBase
{
    private string title;
    private bool selected;
    private List<Vector3> position = new List<Vector3>();
    private List<Quaternion> rotation = new List<Quaternion>();
    private float moveSpeed;
    //private float rotationSpeed;
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
    public List<Vector3> Position
    {
        get { return this.position; }
        set { this.position = value; }
    }

    public List<Quaternion> Rotation
    {
        get { return this.rotation; }
        set { this.rotation = value; }
    }
    public float MoveSpeed
    {
        get { return this.moveSpeed; }
        set { this.Set<float>(ref moveSpeed, value, "MoveSpeed"); }
    }
    //public float RotationSpeed
    //{
    //    get { return this.rotationSpeed; }
    //    set { this.Set<float>(ref rotationSpeed, value, "RotationSpeed"); }
    //}
    public string SceneName
    {
        get { return this.sceneName; }
        set { this.Set<string>(ref sceneName, value, "SceneName"); }
    }
}
