using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Asynchronous;
using System;

/// <summary>
/// 加载界面的vm
/// </summary>
public class LoadingViewModel : ViewModelBase
{
    private string scenceName;
    private bool isOpenMainUI;
    private LoadingProgressBar progressBar = new LoadingProgressBar();
    private InteractionRequest interClose;
    private SimpleCommand loadingCmd;
    /// <summary>
    /// 打开主UI界面
    /// </summary>
    public bool IsOpenMainUI
    {
        get { return this.isOpenMainUI; }
        set { this.Set<bool>(ref isOpenMainUI, value, "IsOpenMainUI"); }
    }
    /// <summary>
    /// 加载场景名
    /// </summary>
    public string ScenceName
    {
        get { return this.scenceName; }
        set { this.Set<string>(ref scenceName, value, "ScenceName"); }
    }
    public ICommand LoadingCmd
    {
        get { return this.loadingCmd; }
    }

    public LoadingProgressBar ProgressBar
    {
        get { return this.progressBar; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }

    public LoadingViewModel()
    {
        this.interClose = new InteractionRequest(this);
        loadingCmd = new SimpleCommand(() =>
          {
              LoadScene();
          }
          );
    }

    /// <summary>
    /// 开始加载界面
    /// </summary>
    public void LoadScene()
    {
        ProgressTask<float> task = new ProgressTask<float>(new Func<IProgressPromise<float>, IEnumerator>(DoLoadScene));
        task.OnPreExecute(() =>
        {
            //任务开始时
            this.progressBar.Enable = true;
            this.ProgressBar.Tip = "Loading...";
        }).OnProgressUpdate(progress =>
        {
            //任务进行中
            this.ProgressBar.Progress = progress;// 进度值更新 
        }).OnPostExecute(() =>
        {
            //任务完成时
            this.ProgressBar.Tip = "";
        }).OnFinish(() =>
        {
            //任务结束
            this.progressBar.Enable = false;
            this.InterClose.Raise();// 发送关闭窗口请求 
            //跳转场景后打开主界面
            if(this.IsOpenMainUI)
                LoxodonWindowCtr.Instance.OpenWindow<MainUIWindow>(Global.prefab_MainWindow);
        }).Start();
    }
    protected IEnumerator DoLoadScene(IProgressPromise<float> promise)
    {
        if(!string.IsNullOrEmpty( this.ScenceName))
        {
            #region 场景的加载
            //SceneManager manager = new SceneManager();
            AsyncOperation op = SceneManager.LoadSceneAsync(this.ScenceName);  
            while (!op.isDone)
            {
                promise.UpdateProgress(op.progress);
                yield return null;
            }
            #endregion

            #region 资源的加载
            //ResourceRequest request = Resources.LoadAsync<GameObject>("Scenes/TestScence");
            //while (!request.isDone)
            //{
            //    promise.UpdateProgress(request.progress);
            //    yield return null;
            //}
            //GameObject sceneTemplate = (GameObject)request.asset;
            //GameObject.Instantiate(sceneTemplate);
            #endregion

            promise.UpdateProgress(1f);
            promise.SetResult();
        }
    }
}
