using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 向量辅助功能
/// </summary>
public static class PositionAndRotationExtension
{
    /// <summary>
    /// 从字符串转换为相机位置vector3的辅助方法
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Vector3 ConvertVectr3ByString(string str)
    {
        if (str.Length <= 0)
            return Vector3.zero;
        str=str.Replace("(", string.Empty);
        str= str.Replace(")", string.Empty);
        string[] tmp_sValues = str.Trim(' ').Split(',');
        if (tmp_sValues != null && tmp_sValues.Length == 3)
        {
            float tmp_fX = float.Parse(tmp_sValues[0]);
            float tmp_fY = float.Parse(tmp_sValues[1]);
            float tmp_fZ = float.Parse(tmp_sValues[2]);

            return new Vector3(tmp_fX, tmp_fY, tmp_fZ);
        }
        return Vector3.zero;
    }
    /// <summary>
    /// 从字符串转换为旋转角度quaternion的辅助方法
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Quaternion ConvertQuaternonByString(string str)
    {
        if (str.Length <= 0)
            return Quaternion.identity;
        str = str.Replace("(", string.Empty);
        str = str.Replace(")", string.Empty);
        string[] tmp_sValues = str.Trim(' ').Split(',');
        if (tmp_sValues != null && tmp_sValues.Length == 4)
        {
            float tmp_fX = float.Parse(tmp_sValues[0]);
            float tmp_fY = float.Parse(tmp_sValues[1]);
            float tmp_fZ = float.Parse(tmp_sValues[2]);
            float tmp_fH = float.Parse(tmp_sValues[3]);

            return new Quaternion(tmp_fX, tmp_fY, tmp_fZ, tmp_fH);
        }
        return Quaternion.identity;
    }
}
