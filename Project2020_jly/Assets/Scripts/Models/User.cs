using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用户类(全局只有一个代表当前用户)
/// </summary>
public  class User
{
    private static readonly User instance = new User();
    private string passWord;
    private string name;
    private bool isAdmin;

    public static User Instance
    {
        get { return instance; }
    }
    public string PassWord
    {
        get { return passWord; }
    }
    public string Name
    {
        get { return name; }
    }
    /// <summary>
    /// 判断当前的用户是否为管理员
    /// </summary>
    public bool IsAdmin
    {
        get { return isAdmin; }
    }
    public User()
    { }
    public void Init(string name,string passWord,bool isAdmin)
    {
        this.name = name;
        this.passWord = passWord;
        this.isAdmin = isAdmin;
    }
    public void UpdateAdministration(bool isAdmin)
    {
        this.isAdmin = isAdmin;
    }
}
