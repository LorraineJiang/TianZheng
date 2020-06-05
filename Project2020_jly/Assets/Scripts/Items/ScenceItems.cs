using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 场景中的物体，可以显示缩略图
/// 一般用于建筑物这种小图显示信息的物体
/// </summary>
public class ScenceItems : ItemBase, IClickItem
{
    #region
    [Header("自由相机"),Tooltip("当相机为空时，则不会显示小窗口")]
    public Camera mainCamera;     
    [Header("第一人称相机"),Tooltip("当相机为空时，则不会显示小窗口")]
    public Camera firstCamera;

    [Header("是否有缩略窗口显示")]
    public bool hasMiniInfoWindow = false;
    [Header("悬浮框的预制体")]
    public GameObject miniWinPrefab;
    [Header("悬浮框的最远显示距离")]
    public float ShowMaxDistance = 80f;
    [Header("悬浮框的最近显示距离")]
    public float ShowMinDistance = 40f;
    [Header("显示的位置点")]
    public Transform showPoint;
    [Header("悬浮框的偏移")]
    public Vector3 offset = new Vector3(0, 15, 0);
    [Header("跳转的场景名")]
    public string sceneName;

    private bool isMainCameraShow;  //自由相机模式下，小窗口显示，hasMiniInfoWindow=true时才适用
    private bool isFirstCameraShow; //第一人称模式下，小窗口显示，hasMiniInfoWindow=true时才适用
    //是否能够显示缩略窗口
    private bool canShowMiniWindow;    
    //悬浮框物体
    private GameObject window;
    #endregion

    private void Start()
    {
        //判断是否有小窗口功能，有则实例化且不显示
        if (hasMiniInfoWindow)
        {
            window = Instantiate(miniWinPrefab, showPoint.position + offset, showPoint.rotation, showPoint);
            window.transform.localScale = new Vector3(1, 0, 1);
            window.SetActive(false);
        }
        //当没有数据记录时不用显示小窗和创建报警粒子
        if (SqliteCtr.DbHelper.ExitItem("BaseInfo", "id", base.id.ToString()))
        {
            canShowMiniWindow = true;
            SetAlert();     //判断是否要显示报警粒子
        }
        StartCoroutine(IEShowMiniWin());
    }

    /// <summary>
    /// 判断物体是否是报警物体，如果是则创建报警粒子
    /// </summary>
    private void SetAlert()
    {
        //SqliteCtr.DbHelper.SelectSpecificData以ID查找表中指定数据
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectSpecificData("BaseInfo", "id", base.id.ToString());
        //用while读取数据，直到将所有数据读完
        while (SqliteCtr.Instance.Reader.Read())
        {
            string relativeTable = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("relativetable"));
            //TODO 根据关联表名来判断？
            if (relativeTable == "AlertItem")
            {
                ScenceManager.Instance.CreateAlert(this.gameObject);
            }
        }
    }

    public void ClickCall()
    {
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectSpecificData("BaseInfo", "id", base.id.ToString());
        string id = base.id.ToString();
        string name = string.Empty;
        string info = string.Empty;
        string imagePath = string.Empty;
        string relativeTable = string.Empty;
        string relativeKey = string.Empty;
        string relativeKeyValue = string.Empty;
        string pragma = string.Empty;
        string type = string.Empty;
        string relativeInterface = string.Empty;
        while (SqliteCtr.Instance.Reader.Read())
        {
            //GetOrdinal返回的是一个从0开始的列序号
            name = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
            info = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("info"));
            imagePath = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("image"));
            relativeTable = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("relativetable"));
            relativeKey = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("relativekey"));
            relativeKeyValue = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("relativekeyvalue"));
            pragma= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("pragma"));
            type= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("type"));
            relativeInterface = SqliteCtr.Instance.Reader.GetValue(SqliteCtr.Instance.Reader.GetOrdinal("interface")).ToString();
        }
        LoxodonWindowCtr.Instance.OpenWindow<InformationWindow>(Global.prefab_InformationWindow).SetModel(id, name, info, imagePath, relativeTable, relativeKey, relativeKeyValue, pragma, type, relativeInterface, this.gameObject);
    }

    public void ClickCall<T>(T t1)
    {}

    /// <summary>
    /// 悬浮框显示协程
    /// 主相机和第一人称会显示小窗口
    /// </summary>
    /// <returns></returns>
    IEnumerator IEShowMiniWin()
    {
        while (true)
        {
            //自由视角
            if (mainCamera != null && mainCamera.gameObject.activeSelf && hasMiniInfoWindow)
            {
                CheckMiniWindowShow(mainCamera, ref isMainCameraShow);
            }
            //第一人称视角
            else if (firstCamera != null && firstCamera.gameObject.activeSelf && hasMiniInfoWindow)
            {
                CheckMiniWindowShow(firstCamera, ref isFirstCameraShow);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 检测小窗是否打开
    /// </summary>
    /// <param name="c"></param>
    /// <param name="show"></param>
    private void CheckMiniWindowShow(Camera c, ref bool show)
    {
        if (!canShowMiniWindow) 
            return;
        float distance = Vector3.Distance(c.transform.position, transform.position);
        if (distance <= ShowMaxDistance && distance > ShowMinDistance)
        {
            //距离满足要求的前提下且对应的自由视角或第一人称视角没有在显示的情况下
            if (!show)
            {
                show = true;
                window.SetActive(true);
                window.transform.DOScaleY(1, 0.1f);
                //根据id去读取对应的数据
                SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectSpecificData("BaseInfo", "id", base.id.ToString());
                while (SqliteCtr.Instance.Reader.Read())
                {
                    string name = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
                    string info = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("info"));
                    string image = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("image"));
                    window.GetComponent<MiniShowWindow>()?.SetData(name, info, image);
                }
            }
        }
        else
        {
            if (show)
            {
                show = false;
                window.transform.DOScaleY(0, 0.1f).OnComplete(() => { window.SetActive(false); });
            }
        }

    }

    /// <summary>
    /// 刷新小窗口内容
    /// </summary>
    public void RefreshMiniWindow()
    {
        //当前信息没有记录时，则不显示小窗口
        if (!SqliteCtr.DbHelper.ExitItem("BaseInfo", "id", base.id.ToString()))
        {
            canShowMiniWindow = false;
            window.SetActive(false);
            return;
        }
        else
        {
            canShowMiniWindow = true;
        }
        if (window.activeSelf)
        {
            //根据id去读取对应的数据，并将读取的数据装入小窗口中 
            SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectSpecificData("BaseInfo", "id", base.id.ToString());
            while (SqliteCtr.Instance.Reader.Read())
            {
                string name = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
                string info = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("info"));
                string image = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("image"));
                window.GetComponent<MiniShowWindow>()?.SetData(name, info, image);
            }
        }
    }
}
