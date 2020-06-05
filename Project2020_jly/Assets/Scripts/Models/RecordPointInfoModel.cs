using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;

/// <summary>
/// 兴趣点相关信息类
/// </summary>
public class RecordPointInfoModel:ViewModelBase
{
    /// <summary>
    /// 标记点名称
    /// </summary>
    private string title;
    /// <summary>
    /// 记录点位置
    /// </summary>
    private Vector3 position;
    /// <summary>
    /// 记录点旋转角度
    /// </summary>
    private Quaternion rotation;
    /// <summary>
    /// 场景名
    /// </summary>
    private string sceneName;

    public string Title
    {
        get { return title; }
        set { this.Set<string>(ref title, value, "Title"); }
    }

    public Vector3 Position
    {
        get { return position; }
        set { this.Set<Vector3>(ref position, value, "Position"); }
    }
    public Quaternion Rotation
    {
        get { return rotation; }
        set { this.Set<Quaternion>(ref rotation, value, "Rotation"); }
    }
    public string SceneName
    {
        get { return sceneName; }
        set { this.Set<string>(ref sceneName, value, "SceneName"); }
    }

    public RecordPointInfoModel()
    {
        
    }
    public RecordPointInfoModel(string title, Vector3 position, Quaternion rotation,string sceneName)
    {
        this.title = title;
        this.position = position;
        this.rotation = rotation;
        this.sceneName = sceneName;
    }
}
