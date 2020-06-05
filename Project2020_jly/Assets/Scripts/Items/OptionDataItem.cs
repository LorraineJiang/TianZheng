using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自定义的下拉列表数据类
/// </summary>
public class OptionDataItem : Dropdown.OptionData
{
    public OptionDataItem() : base()
    {}
    public OptionDataItem(string text) : base(text)
    { }

    public OptionDataItem(string text, string content) : base(text)
    {
        this.relateTable = content;
    }
    public OptionDataItem(string text, string content,string key) : base(text)
    {
        this.relateTable = content;
        this.relateTableKey = key;
    }
    public OptionDataItem(Sprite image) : base(image)
    { }
    public OptionDataItem(string text, Sprite image) : base(text, image)
    { }

    public string relateTable { get; set; }

    public string relateTableKey { get; set; }
}
