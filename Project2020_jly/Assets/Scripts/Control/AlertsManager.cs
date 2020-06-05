using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 报警数据管理类，持有所有的报警数据
/// </summary>
public class AlertsManager
{
    private static readonly AlertsManager instance = new AlertsManager();

    public static AlertsManager Instance
    {
        get { return instance; }
    }
    /// <summary>
    /// 报警数据集合
    /// </summary>
    public List<AlertMessageModel> Alerts = new List<AlertMessageModel>();

    /// <summary>
    /// 增加报警数据
    /// </summary>
    /// <param name="model">报警数据</param>
    public void AddorRemoveItem(AlertMessageModel model)
    {
        //报警激活时
        if (model.State)
        {
            if (Alerts.Count <= 0)
            {
                Alerts.Add(model);
            }
            else
            {
                bool exist = Alerts.Exists((listItem) =>
                 {
                     if (listItem.AlertObjName == model.AlertObjName)
                         return true;
                     else
                         return false;
                 });
                if (!exist)
                {
                    Alerts.Add(model);
                }
            }
        }
        //报警不激活时
        else
        {
            if (Alerts.Count > 0)
            {
                for (int i = 0; i < Alerts.Count; i++)
                {
                    if (Alerts[i].AlertObjName == model.AlertObjName)
                    {
                        Alerts.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
