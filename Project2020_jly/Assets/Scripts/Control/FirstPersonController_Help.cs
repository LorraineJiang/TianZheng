using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 辅助第一人称相机点击功能
/// </summary>
public class FirstPersonController_Help : MonoBehaviour
{
    private Camera mCamera;
    private Ray mRay;
    private RaycastHit mHit;
    private RigidbodyFirstPersonController firstC;
    private void Start()
    {
        mCamera = GetComponentInChildren<Camera>();
        firstC = gameObject.GetComponent<RigidbodyFirstPersonController>();
    }
    void Update()
    {
        CheckClick();
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!firstC.enabled)
                return;
            if (UGUIMenu.Instance.IsShow)
            {
                UGUIMenu.Instance.DetectorCloseMenu();
            }
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
}
