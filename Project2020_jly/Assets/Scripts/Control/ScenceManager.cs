using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using System;
using DG.Tweening;

/// <summary>
/// 场景控制类，控制场景中相应的相机（包括小相机）
/// 全局的按键监听写在此处
/// </summary>
public class ScenceManager : MonoBehaviour
{
    #region
    [Tooltip("第一人称相机")]
    public GameObject firstPersonCamer;
    [Tooltip("驾驶视角相机")]
    public GameObject driveCamera;
    [Tooltip("场景中的小地图")]
    public bl_MiniMap miniMap;
    [Tooltip("打开关闭小地图")]
    public GameObject allMap;
    //使用单例
    private static ScenceManager instance;
    //显示地上的层
    private bool isShowUpgroundLayer = true;
    //场景中所有的可高亮物体
    private List<HighlighterController> allHighlighter = new List<HighlighterController>();
    //选中的高亮物体
    private List<HighlighterController> selectedArray = new List<HighlighterController>();
    //小地图上的标记图标物体
    [HideInInspector]
    public List<GameObject> minimapIcon = new List<GameObject>();
    //地图上的物体
    private ItemBase[] items;
    //退出窗口
    private AlertDialog exitDialog;

    /**测试报警使用例子，无用时删除****/
    [Header("测试报警物体")]
    public string TestAlertObj;
    [Header("测试报警内容")]
    public string TestAlertContent;
    /*********************************/

    public ItemBase[] Items
    {
        get { return items; }
    }

    public List<HighlighterController> SelectedObject
    {
        get { return selectedArray; }
    }
    public List<HighlighterController> AllHighligherObject
    {
        get { return allHighlighter; }
    }

    public static ScenceManager Instance
    {
        get { return instance; }
    }
    #endregion

    /// <summary>
    /// 最早开始执行，且仅执行一次
    /// </summary>
    private void Awake()
    {
        instance = this;
        driveCamera.SetActive(false);
        //获取所有挂ItemBase的物体 TODO FindObjectsOfType效率低
        items = FindObjectsOfType<ItemBase>();
    }

    /// <summary>
    /// 第二个开始，有可能与Start()同时执行，每次初始化时都会再执行一次该方法
    /// </summary>
    private void OnEnable()
    {
        GetAllHighlighter();
    }

    /// <summary>
    /// 每帧更新
    /// </summary>
    private void Update()
    {
        KeyBoardEvent();
    }

    /// <summary>
    /// 获得场景中所有的高亮物体
    /// </summary>
    public void GetAllHighlighter()
    {
        allHighlighter.Clear();
        //TODO FindObjectsOfType效率低
        allHighlighter = new List<HighlighterController>(FindObjectsOfType<HighlighterController>());
    }

    /// <summary>
    /// 应用于全局的按钮操作功能
    /// </summary>
    private void KeyBoardEvent()
    {
        //esc按键弹出退出窗口/停止巡航窗口
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MainCameraController.Instance.isNavigation)
            {
                StopNavigationTip();
            }
            else
            {
                MainCameraController.Instance.mIsActive = false;
                MainCameraController.Instance.mIsActiveRaycster = false;
                ShowExitWindow();
            }
        }
        //X按键打开关闭小地图
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!allMap)
                return;
            else
            {
                if (allMap.transform.localScale.x != 0)
                    allMap.transform.DOScaleX(0, 0.2f);
                else
                    allMap.transform.DOScaleX(1, 0.2f);
            }
        }
        /*****测试报警使用例子，无用时删除*****/
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AlertMessageModel alertMess = new AlertMessageModel() { AlertObjName = TestAlertObj, State = true, AlertContent = TestAlertContent };
            MessageCenter.Instance.Publish<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(), alertMess);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            AlertMessageModel alertMess = new AlertMessageModel() { AlertObjName = TestAlertObj, State = false, AlertContent = TestAlertContent };
            MessageCenter.Instance.Publish<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(), alertMess);
        }
#endif
        /*************************************/
    }

    /// <summary>
    /// 修改相机的显示层(层目前是和UpGround 相关)
    /// TODO：后期有透视的功能，可以选择小地图不同的层来看内部
    /// </summary>
    public void ChangeLayerToCamera()
    {
        if (isShowUpgroundLayer)
        {
            MainCameraController.Instance.gameObject.GetComponent<Camera>().cullingMask = MainCameraController.Instance.gameObject.GetComponent<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("UpGround"));
            firstPersonCamer.GetComponentInChildren<Camera>().cullingMask = firstPersonCamer.GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("UpGround"));
            driveCamera.GetComponentInChildren<Camera>().cullingMask = driveCamera.GetComponentInChildren<Camera>().cullingMask & ~(1 << LayerMask.NameToLayer("UpGround"));
            isShowUpgroundLayer = false;
        }
        else
        {
            MainCameraController.Instance.gameObject.GetComponent<Camera>().cullingMask = MainCameraController.Instance.gameObject.GetComponent<Camera>().cullingMask | (1 << LayerMask.NameToLayer("UpGround"));
            firstPersonCamer.GetComponentInChildren<Camera>().cullingMask = firstPersonCamer.GetComponentInChildren<Camera>().cullingMask | (1 << LayerMask.NameToLayer("UpGround"));
            driveCamera.GetComponentInChildren<Camera>().cullingMask = driveCamera.GetComponentInChildren<Camera>().cullingMask | (1 << LayerMask.NameToLayer("UpGround"));
            isShowUpgroundLayer = true;
        }
    }

    /// <summary>
    /// 高亮场景中的物体
    /// </summary>
    /// <param name="objectId">高亮物体id集合</param>
    public void HighlightByObjList(IList<int> objectId)
    {
        StopAllHighlight();
        selectedArray.Clear();//清除所选中的高亮物体
        for (int i = 0; i < allHighlighter.Count; i++)
        {
            if (allHighlighter[i].GetComponent<ItemBase>() != null)
            {
                if (objectId.Contains(allHighlighter[i].GetComponent<ItemBase>().id))
                {
                    allHighlighter[i].ConstantSwitchImmediate();//高亮每一个可高亮的物体
                    selectedArray.Add(allHighlighter[i]); ;//将每一个可高亮的物体添加进已选中物体的List中
                }
            }
        }
    }

    /// <summary>
    /// 选择高亮物体
    /// </summary>
    /// <param name="obj">高亮物体</param>
    /// <param name="isMultiSelected">是否多选</param>
    private void SelectObject(HighlighterController obj, bool isMultiSelected = false)
    {
        //单机左键选中物体，不可多选物体来高亮
        if (!isMultiSelected)
        {
            for (int i = 0; i < allHighlighter.Count; i++)
            {
                if (obj != allHighlighter[i])
                {
                    allHighlighter[i].StopLighting();   //关闭非当前物体的高亮
                    if (selectedArray.Contains(allHighlighter[i]))
                    {
                        selectedArray.Remove(allHighlighter[i]);
                    }
                }
                else
                {
                    allHighlighter[i].ConstantSwitchImmediate();
                    if (!selectedArray.Contains(obj))
                    {
                        selectedArray.Clear();  //清空选中集合
                        selectedArray.Add(obj);     //集合中添加选中物体
                    }
                    else
                    {
                        selectedArray.Remove(obj);  //去除选中的物体
                    }
                }
            }
        }
        //按住Ctrl再点击物体（可多选）
        else
        {
            if (!selectedArray.Contains(obj))
            {
                selectedArray.Add(obj);   //集合中添加选中物体
            }
            else
            {
                selectedArray.Remove(obj);    //集合中移除选中的物体
            }
            obj.ConstantSwitchImmediate();
        }
    }

    /// <summary>
    /// 选择物体
    /// </summary>
    /// <param name="obj">高亮物体</param>
    /// <param name="isMultiSelected">是否多选</param>
    public void SelectObject(GameObject obj, bool isMultiSelected = false)
    {
        HighlighterController highlight = obj.GetComponent<HighlighterController>();
        if (highlight != null)
            SelectObject(highlight, isMultiSelected);
    }

    /// <summary>
    /// 清空选中的物体集合
    /// </summary>
    public void ClearSelectedObj()
    {
        for (int i = 0; i < selectedArray.Count; i++)
        {
            selectedArray[i].StopLighting();
        }
        selectedArray.Clear();
    }

    /// <summary>
    /// 停止所有的高亮物体
    /// </summary>
    public void StopAllHighlight()
    {
        if (allHighlighter.Count <= 0) return;
        for (int i = 0; i < allHighlighter.Count; i++)
        {
            allHighlighter[i].StopLighting();
        }
    }

    /// <summary>
    /// 小地图创建图标
    /// </summary>
    /// <param name="title"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void CreateMiniMapIcon(string title, Vector3 position, Quaternion rotation)
    {
        bl_MMItemInfo info = new bl_MMItemInfo(position);
        info.Size = 15;
        //info.Color = new Color(1, 0, 0, 1);
        info.Effect = ItemEffect.None;
        info.Interactable = true;       //设置坐标点可点击
        bl_MiniMapItem item = miniMap.CreateNewItem(info, title, true,
            () =>
            {
                //当自由相机激活且不属于巡航状态时才可以跳转（从小地图中点击来跳转）
                if (MainCameraController.Instance.gameObject.activeSelf&&!MainCameraController.Instance.isNavigation)
                {
                    MainCameraController.Instance.JumpTo(position, rotation,true,
                        () => { MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true); });
                    if (miniMap.isFullScreen)
                    {
                        miniMap.ToggleSize();//若在小地图展开状态时开始巡航，则小地图会自动收缩回去
                    }
                }
            });
        if (!minimapIcon.Contains(item.gameObject))
        {
            minimapIcon.Add(item.gameObject);
        }
    }

    /// <summary>
    /// 替换小地图图标
    /// </summary>
    /// <param name="title"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void ReplaceMiniMapIcon(string title, Vector3 position, Quaternion rotation)
    {
        RemoveMiniMapIcon(title);
        CreateMiniMapIcon(title, position, rotation);
    }

    /// <summary>
    /// 移除小地图图标
    /// </summary>
    /// <param name="title">移除图标名称（名称是唯一的）</param>
    public void RemoveMiniMapIcon(string title)
    {
        for (int i = minimapIcon.Count-1; i >= 0; i--)
        {
            if (minimapIcon[i].GetComponent<bl_MiniMapItem>().InfoItem == title)
            {
                Destroy(minimapIcon[i]);
                minimapIcon.RemoveAt(i);//移除minimapIcon的list中第i位的元素
                break;
            }
        }
    }

    /// <summary>
    /// 退出游戏窗口
    /// </summary>
    public void ShowExitWindow()
    {
        //这里的false是在弹出这个窗口时，鼠标不能点击除了此窗口外其他地方
        DialogNotification notification = new DialogNotification(R.application_quit, R.application_view_quitTips, R.application_sure, R.application_cancel, false);
        //对话框的回调函数，此函数会在AlertDialog窗口关闭时调用
        Action<DialogNotification> callback = n =>
        {
            //this.showTitleNullCmd.Enabled = true;
            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                Application.Quit(); ///确定则退出游戏并把锁定的camera释放
                MainCameraController.Instance.mIsActive = true;
                MainCameraController.Instance.mIsActiveRaycster = true;
                exitDialog = null; //关闭窗口
            }
            else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
            {
                exitDialog = null; //取消则关闭窗口并把锁定的camera释放
                MainCameraController.Instance.mIsActive = true;
                MainCameraController.Instance.mIsActiveRaycster = true;
            }
        };

        if (exitDialog == null)
        {
            //显示消息框
            exitDialog = AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
            {
                notification.DialogResult = result;
                if (callback != null)
                    callback(notification);
            });
        }
    }

    /// <summary>
    /// 停止巡航提示窗
    /// </summary>
    private void StopNavigationTip()
    {
        DialogNotification notification = new DialogNotification(R.application_tip, R.application_stopNavigation, R.application_sure, R.application_cancel, false);
        //对话框的回调函数
        Action<DialogNotification> callback = n =>
        {
            //Jly:禁止相机移动，且弹出提示框
            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                MainCameraController.Instance.StopNavigation();
                exitDialog = null;
            }
            else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
            {
                exitDialog = null;
            }
        };
        //显示消息框
        if (exitDialog == null)
        {
            //显示消息框
            exitDialog = AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
            {
                notification.DialogResult = result;
                if (callback != null)
                    callback(notification);
            });
        }
    }

    /// <summary>
    /// 搜索场景中的物体（包含所在场景名的）
    /// </summary>
    /// <param name="name">itembase的itemName（中文名称）或者 gameobject的名称</param>
    public void SearchItem(string name)
    {
        ClearSelectedObj();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == name|| items[i].name == name)
            {
                SelectObject(items[i].gameObject);
                //过渡到对应的偏移位置点
                Vector3 destination = items[i].transform.position + items[i].locateOffset;
                if (destination.y < MainCameraController.Instance.minPosition.y)
                    destination.y = MainCameraController.Instance.minPosition.y;
                MainCameraController.Instance.FadeTo(destination, MainCameraController.Instance.transform.rotation,
                    () =>
                    {
                        MainCameraController.Instance.transform.DOLookAt(items[i].transform.position, 0.6f);
                    }, 0.6f);
                break;
            }
        }
    }

    /// <summary>
    /// 动态创建报警粒子
    /// </summary>
    /// <param name="parent">父节点</param>
    /// <param name="groupId">报警组id</param>
    public void CreateAlert(GameObject parent)
    {
        AlertItem alertObj = parent.GetComponentInChildren<AlertItem>(true);
        if (alertObj != null)
        {
            alertObj.alertObjName = parent.name;
        }
        else
        {
            GameObject res = Resources.Load<GameObject>(Global.prefab_Alert);
            GameObject altert = Instantiate(res, parent.transform);
            altert.transform.rotation = Quaternion.identity;
            alertObj = altert.GetComponent<AlertItem>();
            alertObj.alertObjName = parent.name;
            alertObj.mapItem.InfoItem = string.Empty;
            alertObj.Init();
            alertObj.gameObject.SetActive(false);
        }
        alertObj.transform.localPosition = Vector3.zero;
        //return alertObj;
    }

    /// <summary>
    /// 动态删除报警粒子
    /// </summary>
    /// <param name="parent">父物体</param>
    public void DestroyAlert(GameObject parent)
    {
        AlertItem alertObj = parent.GetComponentInChildren<AlertItem>(true);
        if (alertObj != null)
        {
            alertObj.mapItem.DestroyItem(true); //移除小地图图标
            Destroy(alertObj.gameObject);
        }
    }
}
