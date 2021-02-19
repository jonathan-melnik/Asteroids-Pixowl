
using Unity.Burst;
using Unity.Mathematics;

public abstract class ScreenCorners
{
    public static readonly SharedStatic<float3> LowerLeft = SharedStatic<float3>.GetOrCreate<ScreenCorners, LowerLeftKey>();
    public static readonly SharedStatic<float3> UpperRight = SharedStatic<float3>.GetOrCreate<ScreenCorners, UpperRightKey>();

    // Define a Key type to identify IntField
    private class LowerLeftKey { }
    private class UpperRightKey { }
}