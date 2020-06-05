using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using Loxodon.Framework.Observables;

/// <summary>
/// 作业分组界面的viewmodel
/// </summary>
public class GroupViewModel : ViewModelBase
{
    private SimpleCommand addCmd;
    private SimpleCommand closeCmd;
    private InteractionRequest interClose;
    private InteractionRequest<Notification> interSelectNull;

    #region 方法绑定
    public ICommand AddCmd
    {
        get { return this.addCmd; }
    }
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    public InteractionRequest<Notification> InterSelectNull
    {
        get { return this.interSelectNull; }
    }

    public GroupViewModel()
    {
        this.interClose = new InteractionRequest(this);
        this.interSelectNull = new InteractionRequest<Notification>(this);
        this.addCmd = new SimpleCommand(() =>
        {
            AddBtnClick();
        });
        this.closeCmd = new SimpleCommand(() => { 
            this.interClose.Raise();
            MainCameraController.Instance.mIsActiveRaycster = true;
            MainCameraController.Instance.mIsActive = true;
        });
    }
    #endregion

    public void AddBtnClick()
    {
        //获取所有高亮物体的数目
        if (ScenceManager.Instance.SelectedObject.Count <= 0)
        {
            this.interSelectNull.Raise(new Notification(R.view_GroupSetting_selectedNull));
        }
        else
        {
            RecordGroupInfoModel groupInfo = new RecordGroupInfoModel() { 
                Position =MainCameraController.Instance.transform.position,
                Rotation= MainCameraController.Instance.transform.rotation,
                SceneName=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            };
            for (int i = 0; i < ScenceManager.Instance.SelectedObject.Count; i++)
            {
                if (ScenceManager.Instance.SelectedObject[i].GetComponent<ItemBase>() != null &&
                    ScenceManager.Instance.SelectedObject[i].GetComponent<ItemBase>().canGroup)
                {
                    int objId = ScenceManager.Instance.SelectedObject[i].GetComponent<ItemBase>().id;
                    if (!groupInfo.GroupObjects.Contains(objId))
                    {
                        groupInfo.GroupObjects.Add(objId);
                    }
                }
                else
                {
                    this.interSelectNull.Raise(new Notification(R.view_GroupSetting_selectedCantGroup));
                    return;
                }
            }
            LoxodonWindowCtr.Instance.OpenWindow<SaveGroupsWindow>(Global.prefab_GroupPointSettingWindow)
            .SetModel(groupInfo);
        }
    }
}
