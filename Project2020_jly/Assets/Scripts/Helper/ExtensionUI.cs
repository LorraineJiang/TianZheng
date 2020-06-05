using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI拓展功能，使UI持续朝向相机方向
/// </summary>
public class ExtensionUI : MonoBehaviour
{
    /// <summary>
    /// 主相机
    /// </summary>
    public Camera mTarget;
    /// <summary>
    /// 第一人称相机
    /// </summary>
    public Camera mFPSCamera;
    /// <summary>
    /// 驾驶视角相机
    /// </summary>
    public Camera driveCamera;
    /// <summary>
    /// 是否一直朝向
    /// </summary>
    public bool mIsLookAt=true;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LookAtCamera());
    }

    IEnumerator LookAtCamera()
    {
        while (true)
        {
            if (mIsLookAt)
            {
                if (mTarget != null && mTarget.gameObject.activeSelf)
                {
                    this.transform.forward = mTarget.transform.forward;
                }
                else if (mFPSCamera != null && mFPSCamera.gameObject.activeSelf)
                {
                    this.transform.forward = mFPSCamera.transform.forward;
                }
                if(driveCamera != null && driveCamera.gameObject.activeSelf)
                {
                    this.transform.forward = driveCamera.transform.forward;
                }
            }
            //IEnumerator协程必须用yield return来返回，意味着等待直到所有的摄像机和GUI被渲染完成后，用于让一个Object缓慢消失
            yield return new WaitForEndOfFrame();
        }
        
    }
}
