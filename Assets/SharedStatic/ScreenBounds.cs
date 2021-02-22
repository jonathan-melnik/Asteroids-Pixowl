using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    public static readonly SharedStatic<float> Top = SharedStatic<float>.GetOrCreate<ScreenBounds, TopKey>();
    public static readonly SharedStatic<float> Bottom = SharedStatic<float>.GetOrCreate<ScreenBounds, BottomKey>();
    public static readonly SharedStatic<float> Left = SharedStatic<float>.GetOrCreate<ScreenBounds, LeftKey>();
    public static readonly SharedStatic<float> Right = SharedStatic<float>.GetOrCreate<ScreenBounds, RightKey>();
    public static readonly SharedStatic<float> Width = SharedStatic<float>.GetOrCreate<ScreenBounds, WidthKey>();
    public static readonly SharedStatic<float> Height = SharedStatic<float>.GetOrCreate<ScreenBounds, HeightKey>();

    public static void InitializeValues() {
        Vector3 lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        Left.Data = lowerLeft.x;
        Right.Data = upperRight.x;
        Bottom.Data = lowerLeft.z;
        Top.Data = upperRight.z;
        Width.Data = Right.Data - Left.Data;
        Height.Data = Top.Data - Bottom.Data;
    }

    // Define a Key type to identify fields
    private class TopKey { }
    private class BottomKey { }
    private class LeftKey { }
    private class RightKey { }
    private class WidthKey { }
    private class HeightKey { }
}
