using Loxodon.Framework.Messaging;


/// <summary>
/// 基于loxodon框架的消息中心
/// 一般窗口与窗口之间通信，或者其他操作通知窗口时，选择使用消息的形式响应
/// </summary>
public class MessageCenter
{
    private static readonly Messenger instance = Messenger.Default;

    public static Messenger Instance
    {
        get { return instance; }
    }
}
/// <summary>
/// 消息频道
/// 一般每个消息都独立出一个频道用来区分彼此
/// </summary>
public enum MessageChannel
{
    /// <summary>
    /// 添加位置点
    /// </summary>
    AddPositionPoint,
    /// <summary>
    /// 移除位置点
    /// </summary>
    RemovePositionPoint,
    /// <summary>
    /// 视角列表按钮点击
    /// </summary>
    ScenceViewChangeBtn,
    /// <summary>
    /// 改变主界面底部按钮状态
    /// </summary>
    ChangeBottomBtnState,
    /// <summary>
    /// 改变兴趣点按钮状态
    /// </summary>
    ChangePositionBtnState,
    /// <summary>
    /// 位置点列表关闭
    /// </summary>
    ClosePositionWindow,
    /// <summary>
    /// 切换自由相机
    /// </summary>
    ChangeToFreeCamera,
    /// <summary>
    /// 切换第一人称相机
    /// </summary>
    ChangeToFirstCamera,
    /// <summary>
    /// 切换驾驶模式相机
    /// </summary>
    ChangeToDriveCamera,
    /// <summary>
    /// 新增作业分组
    /// </summary>
    AddJobGroup,
    /// <summary>
    /// 移除作业分组
    /// </summary>
    RemoveJobGroup,
    /// <summary>
    /// 关闭作业分组界面
    /// </summary>
    CloseJobGroupWindow,
    /// <summary>
    /// 设置兴趣点
    /// </summary>
    SettingPosition,
    /// <summary>
    /// 设置分组
    /// </summary>
    SettingGroup,
    /// <summary>
    /// 移除巡航点
    /// </summary>
    RemoveNavigationPoint,
    /// <summary>
    /// 启动报警
    /// </summary>
    ActiveAlertItem,
    /// <summary>
    /// 关闭保存跳转窗口
    /// </summary>
    CloseSaveJumpPointWindow,
}
