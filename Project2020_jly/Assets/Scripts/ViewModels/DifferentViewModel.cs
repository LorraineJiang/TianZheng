using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;

/// <summary>
/// 视角切换窗口的vm
/// </summary>
public class DifferentViewModel : ViewModelBase
{
    #region
    private SimpleCommand changeToMainCmd;
    private SimpleCommand changeToFirstCmd;
    private SimpleCommand changeToDriveCmd;
    private SimpleCommand closeCmd;
    private InteractionRequest interChangeToMain;
    private InteractionRequest interChangeToFirst;
    private InteractionRequest interChangeToDrive;
    private InteractionRequest interClose;

    public InteractionRequest InterChangeToMain
    {
        get { return this.interChangeToMain; }
    }
    public InteractionRequest InterChangeToFirst
    {
        get { return this.interChangeToFirst; }
    }
    public InteractionRequest InterChangeToDrive
    {
        get { return this.interChangeToDrive; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }
    #endregion

    public ICommand ChangeToMainCmd
    {
        get { return changeToMainCmd; }
    }
    public ICommand ChangeToFirstCmd
    {
        get { return changeToFirstCmd; }
    }
    public ICommand ChangeToDriveCmd
    {
        get { return changeToDriveCmd; }
    }
    public ICommand CloseCmd
    {
        get { return closeCmd; }
    }

    
    public DifferentViewModel()
    {
        this.interChangeToMain = new InteractionRequest(this);
        this.interChangeToFirst = new InteractionRequest(this);
        this.interChangeToDrive = new InteractionRequest(this);
        this.interClose = new InteractionRequest(this);
        this.changeToMainCmd = new SimpleCommand(() => 
        {
            this.interChangeToMain.Raise();
        });
        this.changeToFirstCmd = new SimpleCommand(() => 
        {
            this.interChangeToFirst.Raise();
        });
        this.changeToDriveCmd = new SimpleCommand(() =>
        {
             this.interChangeToDrive.Raise();
        });
        this.closeCmd = new SimpleCommand(() =>
        {
            interClose.Raise();
        });
    }
}
