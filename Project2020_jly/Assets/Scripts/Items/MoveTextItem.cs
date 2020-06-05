using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 跑马灯滚动的文本
/// </summary>
public class MoveTextItem : MonoBehaviour
{
    public Transform from;
    public Transform to;
    public float moveDelta = 2f;
    [HideInInspector]
    public bool isActive;
    public Text content;

    public void OnPlay(bool state)
    {
        isActive = state;
        if (state)
            transform.localPosition = new Vector3(from.localPosition.x + content.GetComponent<RectTransform>().rect.width, 
                from.localPosition.y, from.localPosition.z);
    }

    private void Update()
    {
        if (isActive)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, to.localPosition, moveDelta);
            if (Vector3.Distance(transform.localPosition, to.localPosition) <= 0.003f)
            {
                transform.localPosition = new Vector3(from.localPosition.x + content.GetComponent<RectTransform>().rect.width, 
                    from.localPosition.y, from.localPosition.z);
            }
        }
    }
}
