using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input); 

    public static Vector3 VerticalZero(this Vector3 mousePosition) => new Vector3(mousePosition.x, 0, mousePosition.z);

    public static Vector3 IsoMousePosition(this Vector3 mousePosition) => new Vector3(0, 0, 0);
}
