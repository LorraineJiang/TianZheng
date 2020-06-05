using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
/// <summary>
/// 报警信息的数据类
/// 调用报警的例子：
/// AlertMessageModel alertMess = new AlertMessageModel() { AlertObjName = "objName", State = true/false, AlertContent = "content" };
/// MessageCenter.Instance.Publish<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(), alertMess);
/// </summary>
public class AlertMessageModel:ViewModelBase
{

    private string alertObjName;
    private bool state;
    private string alertContent;
    /// <summary>
    /// 警报的物体名称
    /// </summary>
    public string AlertObjName
    {
        get { return this.alertObjName; }
        set { this.Set<string>(ref alertObjName, value, "AlertObjName"); }
    }
    /// <summary>
    /// 警报的状态,true为报警，false为消除报警
    /// </summary>
    public bool State
    {
        get { return this.state; }
        set { this.Set<bool>(ref state, value, "State"); }
    }
    /// <summary>
    /// 警报的文本内容（会显示在主UI的跑马灯位置）
    /// </summary>
    public string AlertContent
    {
        get { return this.alertContent; }
        set { this.Set<string>(ref alertContent, value, "AlertContent"); }
    }
}

