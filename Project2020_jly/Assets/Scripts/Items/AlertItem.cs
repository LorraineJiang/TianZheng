using UnityEngine;
using System;

/// <summary>
/// 报警粒子物体
/// TODO 报警的两种可能形式：
/// 1.服务的发送消息，客户端接收并且响应  
/// 2.客户端持续监听服务端的状态，状态改变则发消息通知自身
/// </summary>
public class AlertItem : MonoBehaviour
{
    private ParticleSystem partical;//粒子系统（特效用）
    private IDisposable alertSub;//为IDisposable类型的资源会自动释放
    public bl_MiniMapItem mapItem;
    public string alertObjName;     //报警粒子对应的物体名称，创建时会自动赋上父物体的名称
    private void OnEnable()
    {
        partical = GetComponent<ParticleSystem>();
    }

    //private void Update()
    //{
    //    //TODO 持续监听服务端的状态
    //}

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        mapItem.HideItem();
        //注册相应的报警信息
        alertSub = MessageCenter.Instance.Subscribe<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(),
            (model) =>
            {
                if (model.AlertObjName == alertObjName)
                {
                    //激活/停止粒子物体
                    gameObject.SetActive(model.State);
                    if (partical != null)
                    {
                        if (model.State)
                            partical.Play();
                        else
                            partical.Stop();
                    }
                    //显示/隐藏小地图图示
                    if (mapItem != null)
                    {
                        if (model.State)
                            mapItem.ShowItem();
                        else
                            mapItem.HideItem();
                    }
                }
            });
    }
    private void OnDestroy()
    {
        if (alertSub != null)
        {
            alertSub.Dispose();
            alertSub = null;
        }
    }
}
