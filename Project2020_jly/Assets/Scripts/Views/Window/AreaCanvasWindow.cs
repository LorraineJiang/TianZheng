using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择关卡的ui窗口，没有数据所以不用loxdom框架来做
/// </summary>
public class AreaCanvasWindow : MonoBehaviour
{
    public Button quitButton;
    private AlertDialog existDialog;
    private AreaCameraController areaCamera;
    void Start()
    {
        areaCamera = FindObjectOfType<AreaCameraController>();
        quitButton.onClick.AddListener(() =>
        {
            ShowExitWindow();
            if (areaCamera != null)
                areaCamera.isEnable = false;
        });
    }

    public void ShowExitWindow()
    {
        DialogNotification notification = new DialogNotification(R.application_quit, R.application_view_quitTips, R.application_sure, R.application_cancel, false);
        //对话框的回调函数
        Action<DialogNotification> callback = n =>
        {
            //this.showTitleNullCmd.Enabled = true;
            if (n.DialogResult == AlertDialog.BUTTON_POSITIVE)
            {
                Application.Quit(); //退出游戏
                existDialog = null; //关闭窗口
            }
            else if (n.DialogResult == AlertDialog.BUTTON_NEGATIVE)
            {
                existDialog = null; //关闭窗口
                if (areaCamera != null)
                    areaCamera.isEnable = true;
            }
        };

        if (existDialog == null)
        {
            //显示消息框
            existDialog = AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
            {
                notification.DialogResult = result;
                if (callback != null)
                    callback(notification);
            });
        }
    }
}
