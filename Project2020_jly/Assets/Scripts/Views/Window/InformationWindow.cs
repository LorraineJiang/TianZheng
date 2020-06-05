using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using UnityEngine.Networking;

/// <summary>
/// 建筑信息显示界面
/// </summary>
public class InformationWindow : Window
{
    #region
    public Text title;
    public Text information;
    public Text relativeInformation;
    public RawImage image;
    public Button closeButton;
    public Button modifyButton;
    public Button clearButton;
    public Button enterButton;
    public Button setImgButton;
    public Button clearImgButton;
    public RectTransform monitorButtonPannel;      //监视按钮pannnel
    public RectTransform valveButtonPannel;        //阀门按钮pannel
    private GameObject sceneItem;    //当前信息对应的物体
    private string originTable;
    private string originKeyValue;
    private InformationViewModel viewModel;
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new InformationViewModel();

        BindingSet<InformationWindow, InformationViewModel> bindingSet = this.CreateBindingSet(viewModel);
        bindingSet.Bind(this.title).For(v => v.text).To(vm => vm.Name).OneWay();
        bindingSet.Bind(this.information).For(v => v.text).To(vm => vm.Info).OneWay();
        bindingSet.Bind(this.relativeInformation).For(v => v.text).To(vm => vm.Pragma).TwoWay();
        //TODO 图片的显示

        bindingSet.Bind(this.modifyButton).For(v => v.onClick).To(vm => vm.ModifyCmd);
        bindingSet.Bind(this.clearButton).For(v => v.onClick).To(vm => vm.ClearCmd);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CloseCmd);
        bindingSet.Bind(this.enterButton).For(v => v.onClick).To(vm => vm.EnterCmd);
        bindingSet.Bind(this.setImgButton).For(v => v.onClick).To(vm => vm.SetImgCmd);
        bindingSet.Bind(this.clearImgButton).For(v => v.onClick).To(vm => vm.ClearImgCmd);
        bindingSet.Bind().For(v => v.ClearDialogWindow).To(vm => vm.InterClearDialog);
        bindingSet.Bind().For(v => v.ClearData).To(vm => vm.InterClear);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Bind().For(v => v.OpenFileWindow).To(vm => vm.InterSetImg);
        bindingSet.Bind().For(v => v.ClearImage).To(vm => vm.InterClearImg);
        bindingSet.Bind().For(v => v.EnterScene).To(vm => vm.InterEnter);
        bindingSet.Build();
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = false;
        MainCameraController.Instance.mIsActive = false;
        //设置管理员可以修改
        modifyButton.gameObject.SetActive(User.Instance.IsAdmin);
        clearButton.gameObject.SetActive(User.Instance.IsAdmin);
    }
    #endregion

    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateInfomation());

    }

    /// <summary>
    /// 释放资源、若在设置里改了类型为报警粒子则自动创建/删除、刷新小窗口更新信息
    /// </summary>
    public override void DoDismiss()
    {
        if (ScenceManager.Instance != null)
        {
            //窗口关闭时，报警物体时自动创建/删除 报警粒子，TODO 根据表名来判断？
            if (viewModel.RelativeTable == "AlertItem")
            {
                ScenceManager.Instance.CreateAlert(sceneItem);
            }
            else
            {
                AutoStopAlert();
                ScenceManager.Instance.DestroyAlert(sceneItem);
            }
            //窗口关闭时，刷新小窗口信息
            sceneItem.GetComponent<ScenceItems>()?.RefreshMiniWindow();
        }
        base.DoDismiss();
    }

    /// <summary>
    /// 关闭窗口且释放主相机
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
        MainCameraController.Instance.mIsActiveRaycster = true;
        MainCameraController.Instance.mIsActive = true;
    }

    /// <summary>
    /// 清除数据记录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ClearData(object sender, InteractionEventArgs args)
    {
        if (SqliteCtr.DbHelper.ExitItem("BaseInfo", "id", viewModel.Id))
        {
            SqliteCtr.DbHelper.Delete("BaseInfo", new string[] { "id" }, new string[] { viewModel.Id });
            viewModel.Name = string.Empty;
            viewModel.Info = string.Empty;
            viewModel.Image = string.Empty;
            viewModel.RelativeTable = string.Empty;
            viewModel.RelativeKey = string.Empty;
            viewModel.RelativeKeyValue = string.Empty;
            viewModel.Pragma = string.Empty;
            viewModel.Type = string.Empty;
            viewModel.RelativeInterface = string.Empty;
        }
        viewModel.CloseCmd.Execute(null);
        sceneItem.GetComponent<ScenceItems>()?.RefreshMiniWindow();
    }

    /// <summary>
    /// 检测停止报警特效
    /// </summary>
    private void AutoStopAlert()
    {
        //删除数据时，清除报警特效
        for (int i = 0; i < ScenceManager.Instance.Items.Length; i++)
        {
            if (ScenceManager.Instance.Items[i].id.ToString() == viewModel.Id)
            {
                AlertItem temAlert = ScenceManager.Instance.Items[i].GetComponentInChildren<AlertItem>();
                if (temAlert != null && temAlert.gameObject.activeSelf)
                {
                    AlertMessageModel alertMess = new AlertMessageModel() { AlertObjName = ScenceManager.Instance.Items[i].gameObject.name, State = false, AlertContent = string.Empty };
                    MessageCenter.Instance.Publish<AlertMessageModel>(MessageChannel.ActiveAlertItem.ToString(), alertMess);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <param name="image"></param>
    /// <param name="relativeTable"></param>
    /// <param name="relativeKey"></param>
    /// <param name="relativeKeyValue"></param>
    /// <param name="pragma"></param>
    /// <param name="type"></param>
    /// <param name="relativeInterface"></param>
    /// <param name="obj"></param>
    public void SetModel(string id,string name, string info, string image,
        string relativeTable,string relativeKey,string relativeKeyValue,
        string pragma,string type,string relativeInterface, GameObject obj)
    {
        this.viewModel.Id = id;
        this.viewModel.Name = name;
        this.viewModel.Info = info;
        this.viewModel.Image = image;
        this.viewModel.RelativeTable = relativeTable;
        this.viewModel.RelativeKey = relativeKey;
        this.viewModel.RelativeKeyValue = relativeKeyValue;
        this.viewModel.Pragma = pragma;
        this.viewModel.Type = type;
        this.viewModel.RelativeInterface = relativeInterface;
        this.sceneItem = obj;
        //当物体不包含ScenceItems脚本时或者没有对应的跳转场景名时，不显示进入场景按钮
        if (sceneItem != null && sceneItem.GetComponent<ScenceItems>() != null)
        {
            if (string.IsNullOrEmpty(sceneItem.GetComponent<ScenceItems>().sceneName))
            {
                this.enterButton.gameObject.SetActive(false);
            }
            else
            {
                this.enterButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 更新关联信息的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateInfomation()
    {
        while (true)
        {
            //if (originTable != viewModel.RelativeTable || (originTable == viewModel.RelativeTable &&originKeyValue != viewModel.RelativeKeyValue))
            //{
            //    if (!string.IsNullOrEmpty(viewModel.RelativeTable))
            //    {
            //        originTable = viewModel.RelativeTable;
            //        originKeyValue = viewModel.RelativeKeyValue;
            //    }
            //}
            if (string.IsNullOrEmpty(viewModel.RelativeInterface))
            {
                relativeInformation.text = viewModel.Pragma;
            }
            else
            {
                //实时同步具体接口得到的文本内容  jly
                //HttpPostBehaviour.Instance.SendPost(viewModel.RelativeInterface,
                HttpPostBehaviour.Instance.SendGet(viewModel.RelativeInterface,
                (jsonText) =>
                {
                    //TODO jsonText是json文本，需要转换格式以显示
                    StringBuilder tempBuilder = new StringBuilder();
                    tempBuilder.AppendLine(viewModel.Pragma);
                    tempBuilder.AppendLine(jsonText);
                    if(relativeInformation!=null)
                        relativeInformation.text = tempBuilder.ToString();
                });
            }
            //TODO 关联的是监视器时，需要显示相应的按钮功能
            if (viewModel.RelativeTable == "Monitor")
            {
                monitorButtonPannel.gameObject.SetActive(true);
            }
            else
            {
                monitorButtonPannel.gameObject.SetActive(false);
            }
            //TODO 关联的是阀门时，显示两个操作按钮
            if (viewModel.RelativeTable == "Valve")
            {
                valveButtonPannel.gameObject.SetActive(true);
            }
            else
            {
                valveButtonPannel.gameObject.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// 清除数据提示窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ClearDialogWindow(object sender, InteractionEventArgs args)
    {
        DialogNotification notification = args.Context as DialogNotification;
        var callback = args.Callback;

        if (notification == null)
            return;
        //显示消息框
        AlertDialog.ShowMessage(notification.Message, notification.Title, notification.ConfirmButtonText, null, notification.CancelButtonText, notification.CanceledOnTouchOutside, (result) =>
        {
            notification.DialogResult = result;
            //点击对应选项后的回调函数，该回调在vm中
            if (callback != null)
                callback();
        });
    }

    /// <summary>
    /// 跳转场景
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void EnterScene(object sender, InteractionEventArgs args)
    {
        LoxodonWindowCtr.Instance.OpenWindow<LoadingWindow>(Global.prefab_LoadingWindow)
            .LoadScence(sceneItem.GetComponent<ScenceItems>().sceneName);
        LoxodonWindowCtr.WindowContainer.Clear();
    }

    /// <summary>
    /// 打开文件浏览窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void OpenFileWindow(object sender, InteractionEventArgs args)
    {
        StartCoroutine(GetTexture(SelectDialog.Instance.ShowDialog()));
    }

    /// <summary>
    /// 清除图像数据
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ClearImage(object sender, InteractionEventArgs args)
    {
        image.texture = null;
        viewModel.Image = string.Empty;
    }

    /// <summary>
    /// 得到图片，并且设置图片的协程
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    IEnumerator GetTexture(string url)
    {
        if (string.IsNullOrEmpty(url))
            yield break;
        url = @"file://" + url;
        using (UnityWebRequest www = new UnityWebRequest(url))
        {
            DownloadHandlerTexture downTexture = new DownloadHandlerTexture();
            www.downloadHandler = downTexture;
            yield return www.SendWebRequest();
            if (www.isDone && www.error == null)
            {
                Texture2D img = downTexture.texture;
                //Sprite sprite = Sprite.Create(img, new Rect(0, 0, img.width, img.height), new Vector2(0.5f, 0.5f));
                image.texture = img;
                //TODO 图片的保存方法？
                byte[] data = img.EncodeToPNG();
                viewModel.Image = System.Convert.ToBase64String(data);
            }
        }
    }
}
