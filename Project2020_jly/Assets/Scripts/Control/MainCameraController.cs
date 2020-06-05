using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 主相机的操作类
/// </summary>
public class MainCameraController : MonoBehaviour
{
    #region Field
    private static MainCameraController instance;
    //操作说明窗口
    private InputDescriptWindow intrWin;
    private GUIStyle introStyle;
    //巡航路线
    public LineRenderer mNavigationLine;

    [Header("相机的拖拽速度"), Range(1, 10)]
    public int mSpeed = 8;
    [Header("相机旋转速度"), Range(1, 20)]
    public int mRotationSpeed = 5;
    [Header("相机的前进速度"), Range(1, 10)]
    public int mScaleSpeed = 5;
    [Header("操作的相机")]
    public Camera mCamera;
    [Header("相机上下的仰角阈值"), Range(0, 89)]
    public float minMaxRotation = 85;
    [Header("相机的最小限制点")]
    public Vector3 minPosition = new Vector3(-50, 10, -50);
    [Header("相机的最大限制点")]
    public Vector3 maxPosition = new Vector3(50, 50, 50);
    [Header("相机旋转时的图标")]
    public Texture2D dragTexture;
    [Header("相机的淡入淡出图片")]
    public Image fadeImg;
    //新增巡航点状态
    [HideInInspector]
    public bool isEditorNav;
    //巡航状态
    [HideInInspector]
    public bool isNavigation;
    // 相机操作可操作状态
    [HideInInspector]
    public bool mIsActive = true;
    // 相机检测状态
    [HideInInspector]
    public bool mIsActiveRaycster = true;
    //垂直方向移动的差值
    protected float mDetaMoveY;
    //水平方向移动的差值
    protected float mDetaMoveX;
    //鼠标X轴移动的差值
    protected float mDetaX;
    //鼠标Y轴移动的差值
    protected float mDetaY;
    //鼠标中键移动移动的差值
    protected float mDetaMid;
    //相机在水平面上的旋转角度
    private Quaternion horizontalRotation;
    //相机路径信息
    private CameraPathModel pathModel;

    #region 射线检测相关数据
    protected Ray mRay;
    protected RaycastHit mHit;
    //射线检测时的差值
    private Vector3 deltaVector;
    #endregion

    public static MainCameraController Instance
    {
        get { return instance; }
    }
    #endregion

    private void Awake()
    {
        instance = this;
        InitCamera();
        ChangeAreaLimit();
        
    }

    /// <summary>
    /// FixedUpdate在固定的时间间隔执行，不受游戏帧率的影响,处理Rigidbody的时候最好用FixedUpdate，时间间隔可以在Edit-ProjectSetting-time-Fixedtimestep中更改
    /// </summary>
    private void FixedUpdate()
    {
        CameraFixPosition();
    }

    /// <summary>
    /// 在所有Update函数调用后被调用,用于当物体在Update里移动时，跟随物体的相机可以在LateUpdate里实现
    /// </summary>
    private void LateUpdate()
    {
        CameraControl();
        CameraKeyBoard();
        CameraIconChange();
    }

    /// <summary>
    /// 每帧自动执行一次
    /// </summary>
    private void OnGUI()
    {
        introStyle = new GUIStyle();
        introStyle.fontSize = 20;
        introStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 50, 100, 30), new GUIContent("F1查看操作说明"), introStyle);
    }

    /// <summary>
    /// 初始化相机参数
    /// </summary>
    private void InitCamera()
    {
        if (!PlayerPrefs.HasKey("CameraSpeed"))
        {
            PlayerPrefs.SetInt("CameraSpeed", mSpeed);
        }
        else
        {
            mSpeed = PlayerPrefs.GetInt("CameraSpeed");
        }

        if (!PlayerPrefs.HasKey("CameraRotationSpeed"))
        {
            PlayerPrefs.SetInt("CameraRotationSpeed", mRotationSpeed);
        }
        else
        {
            mRotationSpeed = PlayerPrefs.GetInt("CameraRotationSpeed");
        }

        if (!PlayerPrefs.HasKey("CameraScaleSpeed"))
        {
            PlayerPrefs.SetInt("CameraScaleSpeed", mScaleSpeed);
        }
        else
        {
            mScaleSpeed = PlayerPrefs.GetInt("CameraScaleSpeed");
        }
    }

    /// <summary>
    /// 退出时保存相机参数
    /// </summary>
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("CameraSpeed", mSpeed);
        PlayerPrefs.SetInt("CameraRotationSpeed", mRotationSpeed);
        PlayerPrefs.SetInt("CameraScaleSpeed", mScaleSpeed);
    }

    /// <summary>
    /// 相机的操作接口   鼠标左右中键的操作
    /// </summary>
    protected virtual void CameraControl()
    {
        if (!mIsActive) return;
        #region 鼠标左键操作
        if (Input.GetMouseButtonDown(0))
        {
            CameraMouseLeftClick();
            if (UGUIMenu.Instance.IsShow)
            {
                UGUIMenu.Instance.DetectorCloseMenu();
            }
        }
        // 垂直和水平移动镜头
        if (Input.GetMouseButton(0))
        {
            mDetaY = Input.GetAxis("Mouse Y");
            mDetaMoveX = Input.GetAxis("Mouse X");
            //当前相机的旋转角度，x轴向的角度为0保持在水平面上
            horizontalRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            //让相机在水平面上移动相应的距离
            transform.Translate(horizontalRotation * new Vector3(mSpeed * -mDetaMoveX, 0, mSpeed * -mDetaY), Space.World);
            if (transform.position.z < minPosition.z)
                transform.position = new Vector3(transform.position.x, transform.position.y, minPosition.z);
            if (transform.position.z > maxPosition.z)
                transform.position = new Vector3(transform.position.x, transform.position.y, maxPosition.z);
            if (transform.position.x < minPosition.x)
                transform.position = new Vector3(minPosition.x, transform.position.y, transform.position.z);
            if (transform.position.x > maxPosition.x)
                transform.position = new Vector3(maxPosition.x, transform.position.y, transform.position.z);
        }
        #endregion

        #region 鼠标右键操作
        if (Input.GetMouseButtonDown(1))
        {
            if (!isEditorNav)
            {
                if (User.Instance.IsAdmin)
                {
                    UGUIMenu.Instance.AddItems(R.application_button_settingPosition, () => {
                        MessageCenter.Instance.Publish<bool>(MessageChannel.SettingPosition.ToString(), true); 
                    });
                    UGUIMenu.Instance.AddItems(R.application_Menu_settingNavigation, () => {
                        isEditorNav = true;
                        pathModel = new CameraPathModel();
                    });
                }
                UGUIMenu.Instance.AddItems(R.application_button_settingGroup, () => { MessageCenter.Instance.Publish<bool>(MessageChannel.SettingGroup.ToString(), true); });
            }
            else
            {
                UGUIMenu.Instance.AddItems(R.application_Menu_addNavigation, () => { 
                    AddNavigationPathData(pathModel);
                });
                UGUIMenu.Instance.AddItems(R.application_complete, () => {
                    if (pathModel.Position.Count <= 0)
                    {
                        //巡航点为空时
                        pathModel.Dispose();
                        pathModel = null;
                        isEditorNav = false;
                    }else
                        LoxodonWindowCtr.Instance.OpenWindow<SaveNavigationWindow>(Global.prefab_SaveNavigationWindow).SetModel(pathModel);
                });
            }
            UGUIMenu.Instance.Show(LoxodonWindowCtr.WindowContainer.gameObject.transform, true);
        }
        #endregion

        #region 鼠标中键的操作
        if (Input.GetMouseButton(2))
        {
            CameraRotattion();
        }
        mDetaMid = Input.GetAxis("Mouse ScrollWheel");
        if (!mCamera.orthographic)
        {
            //透视视角时，朝着物体的正方向
            if (mDetaMid != 0)
            {
                float delta_z = mScaleSpeed * mDetaMid * 2;
                transform.Translate(0, 0, delta_z);

                if (transform.position.z < minPosition.z)
                    transform.position = new Vector3(transform.position.x, transform.position.y, minPosition.z);
                if (transform.position.z > maxPosition.z)
                    transform.position = new Vector3(transform.position.x, transform.position.y, maxPosition.z);
                if (transform.position.y < minPosition.y)
                    transform.position = new Vector3(transform.position.x, minPosition.y, transform.position.z);
                if (transform.position.y > maxPosition.y)
                    transform.position = new Vector3(transform.position.x, maxPosition.y, transform.position.z);
                if (transform.position.x < minPosition.x)
                    transform.position = new Vector3(minPosition.x, transform.position.y, transform.position.z);
                if (transform.position.x > maxPosition.x)
                    transform.position = new Vector3(maxPosition.x, transform.position.y, transform.position.z);
            }
        }
        else
        {
            //正交视角，修改相机的size
            if (mDetaMid != 0)
            {
                float changeSize = mCamera.orthographicSize - mDetaMid * mScaleSpeed;
                if (changeSize < 0) changeSize = 0;
                mCamera.orthographicSize = Mathf.Clamp(changeSize, 0, 100);
            }
        }
        #endregion
    }

    /// <summary>
    /// 相机的键盘操作
    /// </summary>
    protected virtual void CameraKeyBoard()
    {
        //键盘按下F1显示提示信息
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (intrWin == null)
                intrWin = LoxodonWindowCtr.Instance.OpenWindow<InputDescriptWindow>(Global.prefab_InputIntroductionWindow);
            else
            {
                intrWin.Close(intrWin, null);
                intrWin = null;
            }
        }
    }

    /// <summary>
    /// 鼠标图片的改变
    /// </summary>
    protected virtual void CameraIconChange()
    {
        if(mIsActive)
        {
            //若鼠标按下中键，则改变鼠标在场景的图片
            if (Input.GetMouseButton(2))
            {
                Cursor.SetCursor(dragTexture, Vector2.zero, CursorMode.ForceSoftware);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            }
        }
    }

    /// <summary>
    /// 修改相机的区域限制,限制相机的可移动区域
    /// </summary>
    public void ChangeAreaLimit()
    {
        if (transform.position.x > 400 && transform.position.x < 2000)
        {
            minPosition = new Vector3(1106f, 10f, 360f);
            maxPosition = new Vector3(1160f, 80f, 420f);
        }
        else if (transform.position.z > -600 && transform.position.z < 600)
        {
            minPosition = new Vector3(-300f, 10f, -400f);
            maxPosition = new Vector3(150f, 80f, 400f);
        }
        else if (transform.position.z > 2000 && transform.position.z < 3000)
        {
            minPosition = new Vector3(-1850f, 10f, 2180f);
            maxPosition = new Vector3(-1330f, 80f, 2700f);
        }
    }

    /// <summary>
    /// 相机左键点击响应方法
    /// </summary>
    protected virtual void CameraMouseLeftClick()
    {
        if (!mIsActiveRaycster) return;
        //若点击到UI对象，则屏蔽
        if (Block3DRaycast.Instance != null && Block3DRaycast.Instance.Is3DObjectsRaycast()) return;
        mRay = mCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mRay, out mHit))
        {
            if (mHit.collider != null)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    //按住ctrl单击则是多选物体
                    ScenceManager.Instance.SelectObject(mHit.transform.gameObject, true);
                }
                else
                {
                    ScenceManager.Instance.SelectObject(mHit.transform.gameObject);
                }
                //得到物体上的点击接口
                IClickItem clickItems = mHit.transform.GetComponent<IClickItem>();
                if (clickItems != null)
                {
                    clickItems.ClickCall();
                }
            }
        }
    }

    /// <summary>
    /// 相机的旋转方法
    /// </summary>
    protected virtual void CameraRotattion()
    {
        mDetaX = Input.GetAxis("Mouse X");
        mCamera.transform.Rotate(Vector3.up, mDetaX * mRotationSpeed, Space.World);     //相机水平方向绕世界坐标轴旋转

        mDetaY = Input.GetAxis("Mouse Y");
        float x = ClampEulerAngle((mCamera.transform.localEulerAngles.x - mDetaY * mRotationSpeed),
            -minMaxRotation, minMaxRotation);   //y轴向的旋转
        mCamera.transform.localEulerAngles = new Vector3(x, mCamera.transform.localEulerAngles.y, mCamera.transform.localEulerAngles.z);
    }

    /// <summary>
    /// 跳转到对应的位置
    /// </summary>
    /// <param name="varPosition">跳转位置</param>
    /// <param name="rotation">跳转位置的角度</param>
    /// <param name="canFade">可以淡入淡出</param>
    /// <param name="callback">跳转回调</param>
    /// <param name="time">跳转时间</param>
    public void JumpTo(Vector3 varPosition, Quaternion rotation,bool canFade=false,System.Action callback=null, float time = 2f)
    {
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), false);
        mIsActive = false;
        mIsActiveRaycster = false;
        if (rotation.eulerAngles.z > 0)
        {
            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
        }
        
        if (Vector3.Distance(mCamera.transform.position, varPosition) > 500)
        {
            //当距离超出850同时允许淡入淡出时：以淡入淡出效果直接到达指定位置
            if (Vector3.Distance(mCamera.transform.position, varPosition) > 850 && canFade)
            {
                FadeTo(varPosition, rotation);
            }
            //500<距离<=850或者不允许淡入淡出时：边移动y轴坐标边转镜头，最后边将镜头转回来边整体移动
            else
            {
                mCamera.transform.DOMoveY(maxPosition.y, time)
                .OnPlay(() =>
                {
                    mCamera.transform.DOLookAt(varPosition, time);
                })
                .OnComplete(() =>
                {
                    mCamera.transform.DOMove(varPosition, time).
                    OnComplete(() =>
                    {
                        mCamera.transform.DORotate(rotation.eulerAngles, time).OnComplete(() =>
                        {
                            ChangeAreaLimit();
                            mIsActive = true;
                            mIsActiveRaycster = true;
                            callback?.Invoke();
                        });
                    });
                });
            }
        }
        else
        {
            //距离为小于30:边转镜头边移动
            if (Vector3.Distance(mCamera.transform.position, varPosition) < 30)
            {
                mCamera.transform.DOMove(varPosition, time).OnPlay(() =>
                {
                    mCamera.transform.DORotate(rotation.eulerAngles, time); 
                }).OnComplete(()=> 
                {
                    mIsActive = true;
                    mIsActiveRaycster = true;
                    callback?.Invoke();
                });
            }
            //30<=距离<=500：先转镜头再移动，最后将镜头转回来
            else
            {
                mCamera.transform.DOLookAt(varPosition, time).OnComplete(() =>
                {
                    mCamera.transform.DOMove(varPosition, time).OnComplete(() =>
                    {
                        mCamera.transform.DORotate(rotation.eulerAngles, time).OnComplete(
                            () =>
                            {
                                ChangeAreaLimit();
                                mIsActive = true;
                                mIsActiveRaycster = true;
                                callback?.Invoke();
                            });
                    });
                });
            }
        }
    }

    /// <summary>
    /// 相机淡入淡出对应的位置
    /// </summary>
    /// <param name="varPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="callback"></param>
    /// <param name="time"></param>
    public void FadeTo(Vector3 varPosition, Quaternion rotation, System.Action callback = null, float time = 1f)
    {
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), false);
        mIsActive = false;
        mIsActiveRaycster = false;
        fadeImg.DOFade(1, time).OnComplete(() =>
         {
             mCamera.transform.position = varPosition;
             mCamera.transform.rotation = rotation;
             ChangeAreaLimit();
             fadeImg.DOFade(0, time).OnComplete(() =>
             {
                 MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true);
                 mIsActive = true;
                 mIsActiveRaycster = true;
                 if (callback != null)
                     callback();
             });
         });
    }

    /// <summary>
    /// 新增巡航路径点
    /// </summary>
    /// <param name="model"></param>
    private void AddNavigationPathData(CameraPathModel model)
    {
        model.Position.Add(this.transform.position);
        model.Rotation.Add(this.transform.rotation);
        model.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 自动修复相机的位置
    /// 相机进入地形内部时，会自动回弹回来
    /// </summary>
    private void CameraFixPosition()
    {
        RaycastHit tempHit;
        //LayerMask mask = 1 << 你需要开启的Layers层
        //LayerMask mask = 0 << 你需要关闭的Layers层
        LayerMask hitLayer = 1 << (LayerMask.NameToLayer("Terria"));    //将所需要的地形层Terria存为hitLayer

        //原方法：public static bool Raycast(Vector3 origin原始位置, Vector3 direction方向, RaycastHit碰撞物的信息, float distance射线距离，int layerMask射线所射层);
        if (Physics.Raycast(transform.position, Vector3.down, out tempHit, 100f, hitLayer))
        {
            //Vector3.down：负y轴方向      向上射线检测是否有地形
            deltaVector = (transform.position - tempHit.point);
            if (deltaVector.y < 1f)
            {
                transform.position = new Vector3(transform.position.x, tempHit.point.y + 1f, transform.position.z);
            }
        }
        else if (Physics.Raycast(transform.position, Vector3.up, out tempHit, 100f, hitLayer))
        {
            //Vector3.up：正y轴方向        向下射线检测是否有地形
            deltaVector = (tempHit.point - transform.position);
            if (deltaVector.y > 1f)
            {
                transform.position = new Vector3(transform.position.x, tempHit.point.y + 1f, transform.position.z);
            }
        }
        else
        {
            //最后默认使用圆检测是否有地形
            //产生一个以position为圆心以3为半径的碰撞体小球，并返回在hitLayer层小球碰到的碰撞体集合
            Collider[] temp = Physics.OverlapSphere(transform.position, 3f, hitLayer);
            if (temp.Length > 0)
            {
                Vector3 closePoint = temp[0].ClosestPoint(transform.position);  //碰撞体最靠近的点
                transform.position = new Vector3(closePoint.x, closePoint.y + 1f, closePoint.z);
            }
        }
    }

    /// <summary>
    /// 角度限制（防止角度旋转的万象锁）
    /// </summary>
    /// <param name="value">欧拉角度</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    protected float ClampEulerAngle(float value,float min,float max)
    {
        float angle = value - 180;
        if (angle > 0)
        {
            angle -= 180;
        }
        else
        {
            angle += 180;
        }
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// 相机巡航
    /// </summary>
    /// <param name="path">相机巡航的数据</param>
    public void StartNavigation(NavigationItemModel path)
    {
        if (path.Position.Count <= 0) return;
        DrawNavigationLine(path, 5);
        isNavigation = true;
        FadeTo(path.Position[0],
            Quaternion.Euler(new Vector3(path.Rotation[0].eulerAngles.x, path.Rotation[0].eulerAngles.y, 0))
            , () => {
                StartCoroutine(Navigation(path, path.Position.Count));
            });
    }

    /// <summary>
    /// 停止相机巡航
    /// </summary>
    public void StopNavigation()
    {
        if (isNavigation)
        {
            StopAllCoroutines();
            DOTween.Clear();    //停止所有dotween
            isNavigation = false;
            mIsActive = true;
            mIsActiveRaycster = true;
            //删除导航点图标
            for (int i = 0; i < mNavigationLine.positionCount; i++)
            {
                ScenceManager.Instance.RemoveMiniMapIcon(string.Format("Point_{0}", i));
            }
            mNavigationLine.positionCount = 0;
            MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), true);
        }
    }

    /// <summary>
    /// 相机巡航的协程
    /// </summary>
    /// <param name="path"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private IEnumerator Navigation(NavigationItemModel path, int count)
    {
        int index = 1;
        Vector3 desPosition = Vector3.zero;
        Quaternion desRotation = Quaternion.identity;
        mIsActive = false;
        mIsActiveRaycster = false;
        MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), false);
        if (index >= count)
        {
            StopNavigation();
            yield break;
        }
        while (true)
        {
            desPosition = path.Position[index];
            desRotation = path.Rotation[index];
            float distance = Vector3.Distance(desPosition, mCamera.transform.position);
            float moveTime = distance / path.MoveSpeed * 0.15f;  //相机巡航需要的时间
            mCamera.transform.DOMove(desPosition, moveTime).OnPlay(() =>
            {
                mIsActive = false;
                mCamera.transform.DORotate(
                    new Vector3(desRotation.eulerAngles.x, desRotation.eulerAngles.y, 0), moveTime);
            });
            yield return new WaitForSeconds(moveTime);
            index++;
            if (index >= count)
            {
                StopNavigation();
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 绘制巡航路线
    /// </summary>
    /// <param name="path">路径信息</param>
    /// <param name="width">路线的宽度</param>
    private void DrawNavigationLine(NavigationItemModel path,float width)
    {
        if (mNavigationLine == null)
        {
            GameObject obj = new GameObject("NavigationLine");
            obj.transform.position = new Vector3(0, 10, 0);
            obj.transform.rotation = Quaternion.identity;
            obj.layer = LayerMask.NameToLayer("MiniMap");
            mNavigationLine = obj.AddComponent<LineRenderer>();
            //在场景中的小地图层绘制巡航线，由于在小地图层绘制，所以运行场景中看不见
            mNavigationLine.startColor = mNavigationLine.endColor = Color.yellow;
            mNavigationLine.material.color = Color.yellow;
        }
        mNavigationLine.startWidth = mNavigationLine.endWidth = width;
        mNavigationLine.positionCount = path.Position.Count;
        for (int i = 0; i < path.Position.Count; i++)
        {
            mNavigationLine.SetPosition(i, path.Position[i]);
            //绘制关键节点图标，节点的名称是 Point_{0}的格式
            ScenceManager.Instance.CreateMiniMapIcon(string.Format("Point_{0}", i),
                path.Position[i], path.Rotation[i]);
        }
    }
}
