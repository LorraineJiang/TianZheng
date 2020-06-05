using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
/// <summary>
/// 物体信息修改窗口
/// </summary>
public class ModifyInformationWindow : Window
{
    public InputField title;
    public Dropdown modelTypeDropDwon;
    //public Dropdown relationItemDropDwon;
    public InputField introduction;
    public InputField informationInput;
    public InputField relationInputField;
    public Button sureButton;
    public Button cancelButton;
    public Button closeButton;


    private ModifyInformationViewModel viewModel;
    protected override void OnCreate(IBundle bundle)
    {
        viewModel = new ModifyInformationViewModel();
        InitModelTypeDropDwon();
        BindingSet<ModifyInformationWindow, ModifyInformationViewModel> bindingSet = this.CreateBindingSet(viewModel);

        bindingSet.Bind(this.title).For(v => v.text, v => v.onEndEdit).To(vm => vm.EditorName).TwoWay();
        bindingSet.Bind(this.introduction).For(v => v.text, v => v.onEndEdit).To(vm => vm.EditorInfo).TwoWay();
        bindingSet.Bind(this.relationInputField).For(v => v.text, v => v.onValueChanged).To(vm =>vm.EditorInterface).TwoWay();
        bindingSet.Bind(this.informationInput).For(v => v.text, v => v.onValueChanged).To(vm => vm.EditorPragma).TwoWay();
        bindingSet.Bind(this.sureButton).For(v => v.onClick).To(vm => vm.SureCmd);
        bindingSet.Bind(this.cancelButton).For(v => v.onClick).To(vm => vm.CancelCmd);
        bindingSet.Bind(this.closeButton).For(v => v.onClick).To(vm => vm.CancelCmd);
        bindingSet.Bind().For(v => v.Close).To(vm => vm.InterClose);
        bindingSet.Bind().For(v => v.ShowTitleEmptyMessage).To(vm => vm.InterTitleEmpty);
        bindingSet.Build();
        modelTypeDropDwon.onValueChanged.AddListener(SelectModel);
        //relationItemDropDwon.onValueChanged.AddListener(SelectRelation);
        //屏蔽主相机射线检测
        MainCameraController.Instance.mIsActiveRaycster = false;
    }
    public override void DoDismiss()
    {
        base.DoDismiss();
    }

    public void SetModel(InformationViewModel model)
    {
        viewModel.Model = model;
        viewModel.EditorName = model.Name;
        viewModel.EditorInfo = model.Info;
        viewModel.EditorType = model.Type;
        viewModel.EditorPragma = model.Pragma;
        viewModel.EditorInterface = model.RelativeInterface;
        if (!string.IsNullOrEmpty(model.RelativeTable))
        {
            for (int i = 0; i < modelTypeDropDwon.options.Count; i++)
            {
                OptionDataItem tempOption = modelTypeDropDwon.options[i] as OptionDataItem;
                if (tempOption.relateTable == model.RelativeTable)
                {
                    modelTypeDropDwon.value = i;
                    break;
                }
            }
            //for (int i = 0; i < relationItemDropDwon.options.Count; i++)
            //{
            //    if (relationItemDropDwon.options[i].text == model.RelativeKeyValue)
            //    {
            //        relationItemDropDwon.value = i;
            //        break;
            //    }
            //}
            viewModel.EditorRelativeTable = model.RelativeTable;
            viewModel.EditorRelativeKey = model.RelativeKey;
            viewModel.EditorRelativeKeyValue = model.RelativeKeyValue;
        }
        else
        {
            //没有数据时，默认选中第一个项
            if (modelTypeDropDwon.options.Count > 0)
            {
                SelectModel(0);
            }
        }
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void Close(object sender, InteractionEventArgs args)
    {
        this.Dismiss();
    }

    /// <summary>
    /// 切换模型下拉列表
    /// </summary>
    /// <param name="index"></param>
    private void SelectModel(int index)
    {
        OptionDataItem temp = modelTypeDropDwon.options[index] as OptionDataItem;
        viewModel.EditorRelativeTable = temp.relateTable;
        viewModel.EditorRelativeKey = temp.relateTableKey;
        viewModel.EditorType = (index + 1).ToString();
        ChangePragmaContent();
        //relationItemDropDwon.value = 0;
        //relationItemDropDwon.RefreshShownValue();
    }

    /// <summary>
    /// 切换关联下拉列表
    /// </summary>
    /// <param name="index"></param>
    //private void SelectRelation(int index)
    //{
    //    viewModel.EditorRelativeKeyValue = relationItemDropDwon.options[index].text;
    //}

    /// <summary>
    /// 初始化关联表下拉列表
    /// </summary>
    private void InitModelTypeDropDwon()
    {
        modelTypeDropDwon.options.Clear();
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.ReadFullTable("Relation");
        while (SqliteCtr.Instance.Reader.Read())
        {
            OptionDataItem optionData = new OptionDataItem();
            optionData.text = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("name"));
            optionData.relateTable = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("tablename"));
            optionData.relateTableKey= SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("tablekey"));
            modelTypeDropDwon.options.Add(optionData);
        }
    }

    /// <summary>
    /// 修改具体参数的文本
    /// </summary>
    private void ChangePragmaContent()
    {
        //修改具体关联的滚动列表项
        //relationItemDropDwon.options.Clear();
        //if (string.IsNullOrEmpty(viewModel.EditorRelativeTable))
        //{
        //    viewModel.EditorRelativeKey = string.Empty;
        //    viewModel.EditorRelativeKeyValue = string.Empty;
        //}
        //else
        //{
        //    SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.ReadColumnValue(viewModel.EditorRelativeTable, viewModel.EditorRelativeKey);
        //    while (SqliteCtr.Instance.Reader.Read())
        //    {
        //        OptionDataItem optionData = new OptionDataItem();
        //        optionData.text = SqliteCtr.Instance.Reader.GetValue(SqliteCtr.Instance.Reader.GetOrdinal(viewModel.EditorRelativeKey)).ToString();
        //        relationItemDropDwon.options.Add(optionData);
        //    }
        //    if (relationItemDropDwon.options.Count > 0)
        //        viewModel.EditorRelativeKeyValue = relationItemDropDwon.options[0].text;
        //    else
        //        viewModel.EditorRelativeKeyValue = string.Empty;
        //}
        //填充模板文本信息
        SqliteCtr.Instance.Reader = SqliteCtr.DbHelper.SelectByConditions("BaseInfo", "*", new string[] { "type" }, new string[] { viewModel.EditorType });
        while (SqliteCtr.Instance.Reader.Read())
        {
            string tempText = SqliteCtr.Instance.Reader.GetString(SqliteCtr.Instance.Reader.GetOrdinal("pragma"));
            string tempId = SqliteCtr.Instance.Reader.GetValue(SqliteCtr.Instance.Reader.GetOrdinal("id")).ToString();
            if (tempId == viewModel.Model.Id)
            {
                viewModel.EditorPragma = tempText;
                return;
            }
            else
            {
                //将最后一个类型的内容作为模板文本填充
                viewModel.EditorPragma = tempText;
            }
        }
    }

    /// <summary>
    /// 名称为空时的提示窗
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ShowTitleEmptyMessage(object sender, InteractionEventArgs args)
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
}
