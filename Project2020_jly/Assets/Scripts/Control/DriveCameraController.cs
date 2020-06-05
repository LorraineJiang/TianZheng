using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class DriveCameraController : MonoBehaviour
{
    #region
    private static DriveCameraController instance;
    [Header("相机旋转速度"), Range(1, 10)]
    public int driveCameraRotationSpeed = 5;
    [Header("相机的前进速度"), Range(1, 30)]
    public int driveCameraMoveSpeed = 15;
    [Header("操作的相机")]
    public Camera driveCamera;
    //[Header("相机上下的仰角阈值"), Range(0, 89)]
    //public float minMaxDriveRotation = 85;
    //Y轴起始值
    public float rotationY = 20F;
    private RigidbodyFirstPersonController firstC;

    #region 射线检测相关数据
    protected Ray driveRay;
    protected RaycastHit driveHit;
    #endregion

    public static DriveCameraController Instance
    {
        get { return instance; }
    }
    #endregion

    private void Awake()
    {
        instance = this;
        InitDriveCamera();

    }

    private void Start()
    {
        driveCamera = GetComponentInChildren<Camera>();
        firstC = gameObject.GetComponent<RigidbodyFirstPersonController>();
    }

    /// <summary>
    /// FixedUpdate在固定的时间间隔执行，不受游戏帧率的影响,处理Rigidbody的时候最好用FixedUpdate，时间间隔可以在Edit-ProjectSetting-time-Fixedtimestep中更改
    /// 按固定时间移动所控制的驾驶Object
    /// </summary>
    /*private void FixedUpdate()
    {
        CameraFixPosition();
    }*/

    void Update()
    {
        DriveCheckClick();
        RestCameraStatus();
        //CameraRotation();
    }

    /// <summary>
    /// 初始化相机参数
    /// </summary>
    private void InitDriveCamera()
    {
        if (!PlayerPrefs.HasKey("DriveCameraRotationSpeed"))
        {
            PlayerPrefs.SetInt("DriveCameraRotationSpeed", driveCameraRotationSpeed);
        }
        else
        {
            driveCameraRotationSpeed = PlayerPrefs.GetInt("DriveCameraRotationSpeed");
        }
        if (!PlayerPrefs.HasKey("DriveCameraMoveSpeed"))
        {
            PlayerPrefs.SetInt("DriveCameraMoveSpeed", driveCameraMoveSpeed);
        }
        else
        {
            driveCameraMoveSpeed = PlayerPrefs.GetInt("DriveCameraMoveSpeed");
        }
    }

    private void RestCameraStatus()
    {
        firstC.movementSettings.ForwardSpeed = driveCameraMoveSpeed;
        firstC.mouseLook.XSensitivity = driveCameraRotationSpeed;
        firstC.mouseLook.YSensitivity = driveCameraRotationSpeed;
    }

    /// <summary>
    /// 左右键点击功能+制作动态UI窗口
    /// </summary>
    private void DriveCheckClick()
    {
        print("~~~~~~~~~~");
        if (Input.GetMouseButtonDown(0))
        {
            if (!firstC.enabled)
                return;
            if (UGUIMenu.Instance.IsShow)
            {
                UGUIMenu.Instance.DetectorCloseMenu();
            }
            driveRay = driveCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(driveRay, out driveHit))
            {
                if (driveHit.collider != null)
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        //按住ctrl单击则是多选物体
                        ScenceManager.Instance.SelectObject(driveHit.transform.gameObject, true);
                    }
                    else
                    {
                        ScenceManager.Instance.SelectObject(driveHit.transform.gameObject);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!firstC.enabled)
                return;
            UGUIMenu.Instance.AddItems(R.application_button_settingGroup, () => { MessageCenter.Instance.Publish<bool>(MessageChannel.SettingGroup.ToString(), true); });
            UGUIMenu.Instance.Show(LoxodonWindowCtr.WindowContainer.gameObject.transform, true);
        }
    }

    /// <summary>
    /// 退出时保存相机参数
    /// </summary>
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("DriveCameraRotationSpeed", driveCameraRotationSpeed);
        PlayerPrefs.SetInt("DriveCameraMoveSpeed", driveCameraMoveSpeed);
    }

    /// <summary>
    /// 相机的旋转方法
    /// </summary>
    /*protected virtual void CameraRotation()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Horizontal") * driveCameraRotationSpeed;
        rotationY += Input.GetAxis("Vertical") * driveCameraRotationSpeed;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }

    /// <summary>
    /// 自动修复相机的位置
    /// 相机进入地形内部时，会自动回弹回来
    /// </summary>
    /*private void CameraFixPosition()
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

    /*
    /// <summary>
    /// 角度限制（防止角度旋转的万象锁）
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected float LimitedEulerAngle(float value,float min,float max)
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
    */


}
