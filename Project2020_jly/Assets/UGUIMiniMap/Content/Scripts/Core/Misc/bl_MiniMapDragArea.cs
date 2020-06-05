using UnityEngine;
using UnityEngine.EventSystems;

public class bl_MiniMapDragArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{


    private bl_MiniMap MiniMap;
    private Vector2 origin;
    private Vector2 direction;
    private Vector2 smoothDirection;
    private bool touched;
    private int pointerID;

    private Texture2D cursorIcon;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        MiniMap = transform.root.GetComponentInChildren<bl_MiniMap>();
        direction = Vector2.zero;
        touched = false;
        cursorIcon = MiniMap.DragCursorIcon;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerDown(PointerEventData data)
    {
        MainCameraController.Instance.mIsActive = false;  //主相机禁用操作
        if (MiniMap == null || !MiniMap.CanDragMiniMap || data.button != PointerEventData.InputButton.Right)
            return;

        if (!touched)
        {
            touched = true;
            pointerID = data.pointerId;
            origin = data.position;
            Cursor.SetCursor(cursorIcon, MiniMap.HotSpot, CursorMode.ForceSoftware);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnDrag(PointerEventData data)
    {
        MainCameraController.Instance.mIsActive = false; //主相机禁用操作
        if (MiniMap == null || !MiniMap.CanDragMiniMap || data.button != PointerEventData.InputButton.Right)
            return;

        if (data.pointerId == pointerID)
        {
            Vector2 currentPosition = data.position;
            Vector2 directionRaw = currentPosition - origin;
            direction = (directionRaw * Time.deltaTime);

            MiniMap.SetDragPosition(direction);
            origin = data.position;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public void OnPointerUp(PointerEventData data)
    {
        MainCameraController.Instance.mIsActive = true;  //主相机启用操作
        if (MiniMap == null || !MiniMap.CanDragMiniMap || data.button != PointerEventData.InputButton.Right)
            return;

        if (data.pointerId == pointerID)
        {
            direction = Vector2.zero;
            touched = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        }
    }
}