using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

/// <summary>
/// 调用postman的接口类
/// </summary>
public class HttpPostBehaviour : MonoBehaviour
{
    private int errorCout = 0;  //http错误的次数
    private static HttpPostBehaviour instance;  //单例

    public static HttpPostBehaviour Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 调用postman的对应的Post接口
    /// </summary>
    /// <param name="interfaceText">接口的文本内容</param>
    /// <param name="action">得到数据后的操作，参数是json字符串</param>
    public void SendPost(string interfaceText,Action<string> action=null)
    {
        if (string.IsNullOrEmpty(interfaceText)) return;
        errorCout = 0;
        StartCoroutine(PostUrl(interfaceText, action));
    }

    /// <summary>
    /// 调用postman的对应的Get接口
    /// </summary>
    /// <param name="interfaceText">接口的文本内容</param>
    /// <param name="action">得到数据后的操作，参数是json字符串</param>
    public void SendGet(string interfaceText, Action<string> action = null)
    {
        if (string.IsNullOrEmpty(interfaceText)) return;
        errorCout = 0;
        StartCoroutine(GetUrl(interfaceText, action));
    }

    /// <summary>
    /// post的协程
    /// </summary>
    /// <param name="url"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator PostUrl(string url, Action<string> action=null)
    {
        #region 固定的数据初始化
        SendData sendData = new SendData();
        sendData.app_key = "1bf5376b82f88384ebe2297327ae2f9fe547dfed";
        sendData.time_stamp = "1551174536";
        sendData.nonce_str = "123456789";
        sendData.sign = "1bf5376b82f88384ebe2297327ae2f9fe547dfed";
        sendData.img_type = "URL";
        #endregion
        //以POST的形式发送消息
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(sendData));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postBytes);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");    //得到的数据是json的格式
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError)
            {
                if (errorCout < 4)
                {
                    //当http错误次数小于4次时，再调一次协程，重新尝试获取数据
                    errorCout++;
                    yield return StartCoroutine(PostUrl(url, action));
                }
                else
                {
                    //当http错误次数大于4次时，报出问题
                    Debug.LogError(webRequest.error);
                }
            }
            else
            {
                if (action != null)
                {
                    //将得到数据传入到委托中
                    action(webRequest.downloadHandler.text);
                }
            }
        }
    }
    /// <summary>
    /// get的协程
    /// </summary>
    /// <param name="url"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator GetUrl(string url, Action<string> action = null)
    {
        #region 固定的数据初始化
        SendData sendData = new SendData();
        sendData.app_key = "1bf5376b82f88384ebe2297327ae2f9fe547dfed";
        sendData.time_stamp = "1551174536";
        sendData.nonce_str = "123456789";
        sendData.sign = "1bf5376b82f88384ebe2297327ae2f9fe547dfed";
        sendData.img_type = "URL";
        #endregion
        //以POST的形式发送消息
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "Get"))
        {
            byte[] getBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(sendData));
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(getBytes);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");    //得到的数据是json的格式
            yield return webRequest.SendWebRequest();
            if (webRequest.isHttpError)
            {
                if (errorCout < 4)
                {
                    //当http错误次数小于4次时，再调一次协程，重新尝试获取数据
                    errorCout++;
                    yield return StartCoroutine(GetUrl(url, action));
                }
                else
                {
                    //当http错误次数大于4次时，报出问题
                    Debug.LogError(webRequest.error);
                }
            }
            else
            {
                if (action != null)
                {
                    //将得到数据传入到委托中
                    action(webRequest.downloadHandler.text);

                }
            }
        }
    }
}

/// <summary>
/// 固定的数据类，可以不做修改
/// </summary>
[System.Serializable]
public class SendData
{
    public string app_key;
    public string time_stamp;
    public string nonce_str;
    public string sign;
    public string img_type;
}
