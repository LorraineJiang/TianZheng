using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 基于UGUI制作的右键菜单
/// 菜单的预制件可以用scroll view来更改制作
/// </summary>
public class UGUIMenu:ScriptableObject
{
    #region
    /// <summary>
    /// 菜单预制体路径
    /// </summary>
    private static string MenuPrefabPath = "Prefab/UGUIMenu";
    private static readonly UGUIMenu instance = ScriptableObject.CreateInstance<UGUIMenu>();
    /// <summary>
    /// 菜单预制体
    /// </summary>
    private static readonly GameObject prefab = Resources.Load<GameObject>(MenuPrefabPath);
    /// <summary>
    /// 当前菜单项内容
    /// </summary>
    private Dictionary<string, Action> items = new Dictionary<string, Action>();
    private Transform parent;
    //当前菜单物体
    private GameObject menuObj;
    //展开状态
    private bool isShow;
    public bool IsShow
    {
        get { return isShow; }
    }
    public GameObject MenuObj
    {
        get { return menuObj; }
    }
    public Transform Parent
    {
        get { return parent; }
    }
    public Dictionary<string, Action> Items
    {
        get { return items; }
    }
    public static UGUIMenu Instance
    {
        get { return instance; }
    }
    #endregion

    /// <summary>
    /// 显示菜单
    /// </summary>
    /// <param name="position">屏幕位置</param>
    /// <param name="refresh">是否刷新按钮列表</param>
    /// <returns></returns>
    private GameObject Show(Vector2 position, bool refresh = false)
    {
        //当菜单已经打开的情况下，只需要更改位置即可
        if (menuObj != null)
        {
            if (isShow)
            {
                menuObj.GetComponent<RectTransform>().anchoredPosition = position;
                if (refresh)
                {
                    Button[] buttons = menuObj.GetComponentsInChildren<Button>(false);
                    for (int i = buttons.Length-1; i >= 0; i--)
                    {
                        Destroy(buttons[i].gameObject);
                    }
                    CreateButtons();
                }
            }
            return menuObj;
        }
        if (prefab == null)
        {
            Debug.LogErrorFormat("the path:{0} doesnt exists,please check out", MenuPrefabPath);
            return null;
        }
        else
        {
            menuObj = Instantiate(prefab, parent);
            menuObj.GetComponent<RectTransform>().anchoredPosition = position;
            CreateButtons();
        }
        //MainCameraController.Instance.mIsActive = false;
        //MainCameraController.Instance.mIsActiveRaycster = false;
        isShow = true;
        return menuObj;
    }

    /// <summary>
    /// 公用接口Show来调用私有Show方法，显示菜单
    /// </summary>
    /// <param name="parent">菜单父节点</param>
    /// <param name="refresh">是否刷新按钮列表</param>
    /// <returns></returns>
    public GameObject Show(Transform parent,bool refresh=false)
    {
        if(this.parent==null)
            this.parent = parent;
        return Show(Input.mousePosition, refresh);
    }

    /// <summary>
    /// 无父物体时的公用接口Show来调用私有Show方法，显示菜单
    /// </summary>
    /// <param name="refresh">是否刷新按钮列表</param>
    /// <returns></returns>
    public GameObject Show(bool refresh = false)
    {
        if(this.parent==null)
           this.parent = FindObjectOfType<Canvas>()?.transform;
        return Show(Input.mousePosition, refresh);
    }

    /// <summary>
    /// 增加菜单项
    /// </summary>
    /// <param name="name">菜单名，不允许同名</param>
    /// <param name="callback">点击回调</param>
    public void AddItems(string name, Action callback)
    {
        if (!items.ContainsKey(name))
        {
            items.Add(name, callback);
        }
        else
        {
            //Debug.Log("exists");
        }
    }

    /// <summary>
    /// 关闭菜单
    /// </summary>
    private void CloseMenu()
    {
        isShow = false;
        //MainCameraController.Instance.mIsActive = true;
        //MainCameraController.Instance.mIsActiveRaycster = true;
        if (menuObj != null)
        {
            Destroy(menuObj);
            menuObj = null;
            items.Clear();
        }
    }

    /// <summary>
    /// 检测关闭菜单(一般用在鼠标左键点击时)
    /// 判断鼠标位置是否在菜单内，如果不在则关闭菜单
    /// </summary>
    public void DetectorCloseMenu()
    {
        if (menuObj != null)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint
                (menuObj.GetComponent<RectTransform>(), Input.mousePosition))
            {
                CloseMenu();
            }
        }
    }

    /// <summary>
    /// 创建菜单中的按钮项
    /// </summary>
    private void CreateButtons()
    {
        Button buttonTemplate = menuObj.GetComponentInChildren<Button>(true);
        foreach (var item in items)
        {
            Button newBtn = Instantiate(buttonTemplate, buttonTemplate.transform.parent);
            newBtn.GetComponentInChildren<Text>().text = item.Key;
            newBtn.onClick.AddListener(
                   () =>
                   {
                       item.Value();
                       CloseMenu();
                   });
            newBtn.gameObject.SetActive(true);
        }
    }
}
