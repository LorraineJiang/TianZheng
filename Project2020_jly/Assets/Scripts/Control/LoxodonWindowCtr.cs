using Loxodon.Framework.Contexts;
using Loxodon.Framework.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 窗口打开控制类
/// </summary>
public class LoxodonWindowCtr
{
    private static WindowContainer windowContainer;
    private static ApplicationContext context;
    private static readonly LoxodonWindowCtr instance = new LoxodonWindowCtr();

    public static LoxodonWindowCtr Instance
    {
        get { return instance; }
    }
    public static ApplicationContext AppContext
    {
        get { return context; }
        set
        {
            if (context == null)
            {
                context = value;
            }
        }
    }

    public static WindowContainer WindowContainer
    {
        get { return windowContainer; }
        set
        {
            if (windowContainer == null)
            {
                windowContainer = value;
            }
        }
    }
    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <typeparam name="T">需要是一个继承Window类</typeparam>
    /// <param name="prefabPath">窗口预制件的路径</param>
    public T OpenWindow<T>(string prefabPath) where T:Window
    {
        T window;
        if (windowContainer.gameObject.GetComponentInChildren<T>() != null)
        {
            window = windowContainer.gameObject.GetComponentInChildren<T>();
        }
        else
        {
            IUIViewLocator locator = context.GetService<IUIViewLocator>();
            window = locator.LoadWindow<T>(windowContainer, prefabPath);
            window.Create();
            ITransition transition = window.Show().OnStateChanged((w, state) =>
            {
                //log.DebugFormat("Window:{0} State{1}",w.Name,state);
            });
        }
        return window;
    }
}
