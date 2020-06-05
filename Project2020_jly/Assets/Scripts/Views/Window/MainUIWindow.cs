using Loxodon.Framework.Binding;
using System;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Views;
using DG.Tweening;
using Loxodon.Framework.Interactivity;
using System.Text;

/// <summary>
/// 主界面的window
/// </summary>
public class MainUIWindow : Window
{
    public Button mPsitionViewBtn;
    public Button mDollyTrackBtn;
    public Button mSettingBtn;
    public Button mDifferentViewBtn;
    public Button mBackBtn;
    public Button mAlertBtn;
    public Button mSearchBtn;
    public Button mAlertButtonPrefab;
    //兴趣点窗口，由于小地图需要使用到数据，所以窗口的vm一直持有，窗口通过隐藏来实现关闭效果
    public RectTransform mPositionWindow;
    //警报时的跑马灯UI界面
    public RectTransform mAlertBarPanel;
    private MainUIViewModel viewModel;

    private IDisposable viewSub;
    private IDisposable closePositionSub;
    private IDisposable settingPositionSub;
    private IDisposable showPositionSub;
    private IDisposable settingGroupsSub;
    private IDisposable bottomStateSub;
    private IDisposable alertBarSub;
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new MainUIViewModel();

        BindingSet<MainUIWindow, MainUIViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.mPsitionViewBtn).For(v => v.onClick).To(vm => vm.PositionViewShowCmd);
        bindingSet.Bind(this.mDifferentViewBtn).For(v => v.onClick).To(vm => vm.ScenceViewOpenCmd);
        bindingSet.Bind(this.mDollyTrackBtn).For(v => v.onClick).To(vm => vm.DollyTrackCmd);
        bindingSet.Bind(this.mSettingBtn).For(v => v.onClick).To(vm => vm.SettingCmd);
        bindingSet.Bind(this.mBackBtn).For(v => v.onClick).To(vm => vm.BackCmd);
        bindingSet.Bind(this.mSearchBtn).For(v => v.onClick).To(vm => vm.SearchCmd);
        #region 绑定本界面的交互动作
        bindingSet.Bind().For(v => v.ShowPositionWindow).To(vm => vm.InterPositionViewShowReq);
        bindingSet.Bind().For(v => v.BackToSelectScene).To(vm => vm.InterBackReq);
        bindingSet.Bind().For(v => v.ClosePositionWindow).To(vm => vm.InterClosePositionViewReq);
        bindingSet.Bind().For(v => v.SearchBtnClick).To(vm => vm.InterSearchReq);
        #endregion

        #region 注册对应的消息

        //注册视角列表展开消息
        viewSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ScenceViewChangeBtn.ToString(), (b) => { 
            viewModel.ScenceViewOpenCmd.Execute(null);
            MessageCenter.Instance.Publish<bool>(MessageChannel.ChangePositionBtnState.ToString(), b);
        });
        //注册兴趣点按钮的状态消息
        showPositionSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangePositionBtnState.ToString(), (b) => 
        { mPsitionViewBtn.interactable = b; });

        //注册位置列表关闭消息
        closePositionSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ClosePositionWindow.ToString(), (b) => { viewModel.PositionViewCloseCmd.Execute(null); });
        //注册兴趣点响应消息
        settingPositionSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.SettingPosition.ToString(), (b) => { viewModel.JumpPointCmd.Execute(null); });
        //注册分组响应消息
        settingGroupsSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.SettingGroup.ToString(), (b) => { viewModel.GroupCmd.Execute(null); });
        //注册底部按钮状态消息（点击底部按钮时打开界面，不激活底部；再关掉界面时，激活底部按钮）
        bottomStateSub = MessageCenter.Instance.Subscribe<bool>(MessageChannel.ChangeBottomBtnState.ToString(), (b) =>
        {
            mPsitionViewBtn.interactable = b;
            mDifferentViewBtn.interactable = b;
            mDollyTrackBtn.interactable = b;
            mSettingBtn.interactable = b;
            mBackBtn.interactable = b;
            mSearchBtn.interactable = b;
        });

        #region 注册警报通知跑马灯
        alertBarSub = MessageCenter.Instance.Subscribe<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(),
            (model) =>
        {
            if (string.IsNullOrEmpty(model.AlertObjName)) 
                return;
            //增加或移除报警数据
            AlertsManager.Instance.AddorRemoveItem(model);
            //跑马灯上的显示文本
            MoveTextItem alertText = mAlertBarPanel.GetComponentInChildren<MoveTextItem>();
            //先清空报警按钮列表数据
            if (mAlertButtonPrefab.transform.parent.childCount > 1)
            {
                for (int i = 1; i < mAlertButtonPrefab.transform.parent.childCount; i++)
                {
                    Destroy(mAlertButtonPrefab.transform.parent.GetChild(i).gameObject);
                }
            }
            alertText.content.text = string.Empty;
            //根据报警的数据来实例化出对应的按钮
            for (int i = 0; i < AlertsManager.Instance.Alerts.Count; i++)
            {
                Button jumpBtn = Instantiate(mAlertButtonPrefab, mAlertButtonPrefab.transform.parent);
                jumpBtn.gameObject.SetActive(true);
                //跳转物体的名称
                string tempName = AlertsManager.Instance.Alerts[i].AlertObjName;
                //注册跑马灯的跳转按钮的功能
                jumpBtn.onClick.AddListener(() =>
                {
                    //当是第一人称或驾驶模式时，切换自由相机
                    if (ScenceManager.Instance.firstPersonCamer.activeSelf)
                    {
                        MainCameraController.Instance.gameObject.SetActive(true);
                        ScenceManager.Instance.firstPersonCamer.SetActive(false);
                        ScenceManager.Instance?.miniMap.SetTarget(MainCameraController.Instance.gameObject);
                        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = MainCameraController.Instance.gameObject.GetComponent<Camera>();
                    }
                    else if(ScenceManager.Instance.driveCamera.activeSelf)
                    {
                        MainCameraController.Instance.gameObject.SetActive(true);
                        ScenceManager.Instance.driveCamera.SetActive(false);
                        ScenceManager.Instance?.miniMap.SetTarget(MainCameraController.Instance.gameObject);
                        ScenceManager.Instance.miniMap.GetComponent<bl_MMCompass>().followCamera = MainCameraController.Instance.gameObject.GetComponent<Camera>();
                    }
                    ScenceManager.Instance.SearchItem(tempName);
                });
                StringBuilder strB = new StringBuilder();
                strB.Append(alertText.content.text);
                strB.Append(string.Format("     {0}", AlertsManager.Instance.Alerts[i].AlertContent));
                alertText.content.text = strB.ToString();
            }
            //跑马灯的显示状态
            if (AlertsManager.Instance.Alerts.Count > 0)
            {
                mAlertBarPanel.gameObject.SetActive(true);
                //TODO 每一次有新的报警都会重新播放滚动效果
                alertText.OnPlay(true);
            }
            else
            {
                mAlertBarPanel.gameObject.SetActive(false);
                alertText.OnPlay(false);
            }
        });
        #endregion

        #endregion

        bindingSet.Build();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void DoDismiss()
    {
        base.DoDismiss();
        if (viewSub != null)
        {
            viewSub.Dispose();
            viewSub = null;
        }
        if (closePositionSub != null)
        {
            closePositionSub.Dispose();
            closePositionSub = null;
        }
        if (settingPositionSub != null)
        {
            settingPositionSub.Dispose();
            settingPositionSub = null;
        }
        if (settingGroupsSub != null)
        {
            settingGroupsSub.Dispose();
            settingGroupsSub = null;
        }
        if (showPositionSub != null)
        {
            showPositionSub.Dispose();
            showPositionSub = null;
        }
        if (bottomStateSub != null)
        {
            bottomStateSub.Dispose();
            bottomStateSub = null;
        }
        if (alertBarSub != null)
        {
            alertBarSub.Dispose();
            alertBarSub = null;
        }
    }

    /// <summary>
    /// 打开兴趣点窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ShowPositionWindow(object sender, InteractionEventArgs args)
    {
        //打开兴趣点列表窗口
        mPositionWindow.gameObject.SetActive(true);
        mPositionWindow.transform.localScale = Vector3.zero;
        mPositionWindow.transform.DOScale(Vector3.one, 0.3f).OnComplete(
            ()=> {
                MessageCenter.Instance.Publish<bool>(MessageChannel.ChangeBottomBtnState.ToString(), false);
                mPositionWindow.GetComponent<JumpPointWindow>().searchInput.text = string.Empty;
                MainCameraController.Instance.mIsActive = false;
                MainCameraController.Instance.mIsActiveRaycster = false;
            });
    }

    /// <summary>
    /// 隐藏兴趣点窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ClosePositionWindow(object sender, InteractionEventArgs args)
    {
        mPositionWindow.transform.DOScale(Vector3.zero, 0.3f).OnComplete(
            ()=> {
                mPositionWindow.gameObject.SetActive(false);
                mPositionWindow.GetComponent<JumpPointWindow>().searchInput.text = string.Empty;
            });
        
    }

    /// <summary>
    /// 返回选择界面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void BackToSelectScene(object sender,EventArgs args)
    {
        this.Dismiss();
        UnityEngine.SceneManagement.SceneManager.LoadScene(Global.scence_shapan);
    }

    /// <summary>
    /// 搜索按钮点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void SearchBtnClick(object sender, EventArgs args)
    {
        LoxodonWindowCtr.Instance.OpenWindow<SearchWindow>(Global.prefab_SearchWindow);
    }
}
