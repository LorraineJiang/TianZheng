using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;

/// <summary>
/// 搜索框对应的viewmodel
/// </summary>
[System.Obsolete("不再使用这个vm")]
public class SearchPanelViewModel : ViewModelBase
{
    private string searchText;
    private SimpleCommand searchCmd;
    private InteractionRequest interSearch;

    public InteractionRequest InterSearch
    {
        get { return this.interSearch; }
    }

    public ICommand SearchCmd
    {
        get { return this.searchCmd; }
    }

    public string SearchText
    {
        get{
            return this.searchText; 
        }
        set {
            this.Set<string>(ref searchText, value, "SearchText");
        }
    }

    public SearchPanelViewModel()
    {
        this.interSearch = new InteractionRequest(this);
        this.searchCmd = new SimpleCommand(() => 
        {
            //ScenceManager.Instance.SearchItem(searchText);
            this.interSearch.Raise();
        });
    }
}
