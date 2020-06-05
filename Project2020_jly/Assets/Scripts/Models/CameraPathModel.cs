using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Observables;
/// <summary>
/// 相机巡航点的数据类
/// </summary>
public class CameraPathModel:ViewModelBase
{
    private string name;
    private ObservableList<Vector3> position = new ObservableList<Vector3>();
    private ObservableList<Quaternion> rotation = new ObservableList<Quaternion>();
    private float moveSpeed = 5;
    //private float rotationSpeed = 5;
    private string sceneName;

    public string Name
    {
        get { return this.name; }
        set { this.Set<string>(ref name, value, "Name"); }
    }
    public ObservableList<Vector3> Position
    {
        get { return this.position; }
        set { this.Set<ObservableList<Vector3>>(ref position, value, "Position"); }
    }
    public ObservableList<Quaternion> Rotation
    {
        get { return this.rotation; }
        set { this.Set<ObservableList<Quaternion>>(ref rotation, value, "Rotation"); }
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
