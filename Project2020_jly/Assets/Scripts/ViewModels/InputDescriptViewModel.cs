using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;

/// <summary>
/// 操作说明窗口的viewmodel
/// </summary>
public class InputDescriptViewModel : ViewModelBase
{
    private SimpleCommand closeCmd;
    private InteractionRequest interClose;
    public ICommand CloseCmd
    {
        get { return this.closeCmd; }
    }
    public InteractionRequest InterClose
    {
        get { return this.interClose; }
    }

    public InputDescriptViewModel()
    {
        closeCmd = new SimpleCommand(() =>
          {
              interClose.Raise();
          });
        interClose = new InteractionRequest(this);
    }
}
