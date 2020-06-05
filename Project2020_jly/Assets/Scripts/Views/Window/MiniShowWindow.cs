using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// 缩略图窗口类
/// </summary>
public class MiniShowWindow:MonoBehaviour
{
    public Text Title;
    public Text Info;
    public Image image;

    public void SetData(string name, string info, string image)
    {
        Title.text = name;
        Info.text = info;
        //Height.text = image;
        //TODO 图片的显示
    }

}
