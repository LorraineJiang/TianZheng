using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;

/// <summary>
/// 工作分组相关信息类
/// </summary>
public class RecordGroupInfoModel : ViewModelBase
{
    private int id;
    private string name;
    private ObservableList<int> groupObjects = new ObservableList<int>();
    private Vector3 position;
    private Quaternion rotation;
    private string sceneName;
    public int Id
    {
        get { return this.id; }
        set { this.Set<int>(ref id, value, "Id"); }
    }
    public string Name
    {
        get { return this.name; }
        set { this.Set<string>(ref name, value, "Name"); }
    }
    public ObservableList<int> GroupObjects
    {
        get { return this.groupObjects; }
        set { this.Set<ObservableList<int>>(ref groupObjects, value, "GroupObjects"); }
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
