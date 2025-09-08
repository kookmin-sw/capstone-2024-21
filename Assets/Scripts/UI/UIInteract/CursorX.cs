using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorX
{
    /// <summary>
    /// <para><b>true</b> : 커서가 화면에 나타나고 자유롭게 움직일 수 있음</para>
    /// <para><b>false</b> : 커서가 화면에서 사라지고 중앙에 고정됨</para>
    /// </summary>
    /// <param name="isFree"></param>
    public static void Free(bool isFree)
    {
        Cursor.visible = isFree;
        Cursor.lockState = isFree ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
